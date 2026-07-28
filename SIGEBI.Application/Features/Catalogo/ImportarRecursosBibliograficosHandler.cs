using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Abstractions.Importacion;
using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Catalogo;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ImportarRecursosBibliograficosHandler
{
    private const long TamanoMaximoBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".csv", ".xlsx", ".xls"
    };

    private static readonly HashSet<string> ContentTypesPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "text/csv",
        "application/csv",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRecursoBibliograficoRepository _recursoRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IImportacionRecursoBibliograficoReader _reader;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ImportarRecursosBibliograficosHandler(
        IUsuarioRepository usuarioRepository,
        IRecursoBibliograficoRepository recursoRepository,
        IEjemplarRepository ejemplarRepository,
        IAutorRepository autorRepository,
        ICategoriaRepository categoriaRepository,
        IImportacionRecursoBibliograficoReader reader,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _recursoRepository = recursoRepository;
        _ejemplarRepository = ejemplarRepository;
        _autorRepository = autorRepository;
        _categoriaRepository = categoriaRepository;
        _reader = reader;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ResultadoImportacionRecursosBibliograficosDto>> HandleAsync(
        ImportarRecursosBibliograficosCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);
        if (!validacion.IsSuccess)
            return ApplicationResult<ResultadoImportacionRecursosBibliograficosDto>.Failure(validacion.Error!);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult<ResultadoImportacionRecursosBibliograficosDto>.Failure(
                "El usuario responsable no fue encontrado.");

        if (usuario.Estado != EstadoUsuario.Activo)
            return ApplicationResult<ResultadoImportacionRecursosBibliograficosDto>.Failure(
                "El usuario responsable no se encuentra activo.");

        var filas = await _reader.LeerAsync(
            command.NombreArchivo,
            command.ContentType,
            command.Contenido,
            cancellationToken);

        if (filas.Count == 0)
            return ApplicationResult<ResultadoImportacionRecursosBibliograficosDto>.Failure(
                "El archivo no contiene filas para importar.");

        var detalles = new List<DetalleImportacionRecursoBibliograficoDto>();
        var filasPreparadas = new List<FilaPreparada>();
        var codigosRecursos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var isbns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var codigosEjemplares = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var autores = new Dictionary<string, Autor>(StringComparer.OrdinalIgnoreCase);
        var categorias = new Dictionary<string, Categoria>(StringComparer.OrdinalIgnoreCase);
        var autoresNuevos = new HashSet<Autor>();
        var categoriasNuevas = new HashSet<Categoria>();

        foreach (var fila in filas)
        {
            try
            {
                var preparada = await PrepararFilaAsync(
                    fila,
                    codigosRecursos,
                    isbns,
                    codigosEjemplares,
                    autores,
                    categorias,
                    autoresNuevos,
                    categoriasNuevas,
                    cancellationToken);

                filasPreparadas.Add(preparada);
                codigosRecursos.Add(preparada.Recurso.CodigoInterno);

                if (!string.IsNullOrWhiteSpace(preparada.Recurso.Isbn))
                    isbns.Add(preparada.Recurso.Isbn);

                if (!string.IsNullOrWhiteSpace(preparada.CodigoInternoEjemplar))
                    codigosEjemplares.Add(preparada.CodigoInternoEjemplar);

                detalles.Add(CrearDetalle(fila, true, preparada.Mensaje));
            }
            catch (ArgumentException ex)
            {
                detalles.Add(CrearDetalle(fila, false, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                detalles.Add(CrearDetalle(fila, false, ex.Message));
            }
        }

        var recursosCreados = filasPreparadas.Count;
        var recursosOmitidos = filas.Count - recursosCreados;
        var ejemplaresCreados = filasPreparadas.Count(fila => fila.CodigoInternoEjemplar is not null);
        var relacionesAutores = filasPreparadas.Sum(fila => fila.Autores.Count);
        var relacionesCategorias = filasPreparadas.Sum(fila => fila.Categorias.Count);

        if (recursosCreados > 0)
        {
            await GuardarImportacionAsync(
                command,
                filas.Count,
                filasPreparadas,
                autoresNuevos,
                categoriasNuevas,
                ejemplaresCreados,
                recursosOmitidos,
                cancellationToken);
        }

        var resultado = new ResultadoImportacionRecursosBibliograficosDto
        {
            TotalFilas = filas.Count,
            RecursosCreados = recursosCreados,
            RecursosOmitidos = recursosOmitidos,
            EjemplaresCreados = ejemplaresCreados,
            AutoresCreados = autoresNuevos.Count,
            CategoriasCreadas = categoriasNuevas.Count,
            RelacionesAutoresCreadas = relacionesAutores,
            RelacionesCategoriasCreadas = relacionesCategorias,
            Detalles = detalles
        };

        return ApplicationResult<ResultadoImportacionRecursosBibliograficosDto>.Success(resultado);
    }

    private async Task<FilaPreparada> PrepararFilaAsync(
        FilaImportacionRecursoBibliografico fila,
        HashSet<string> codigosRecursos,
        HashSet<string> isbns,
        HashSet<string> codigosEjemplares,
        Dictionary<string, Autor> autores,
        Dictionary<string, Categoria> categorias,
        HashSet<Autor> autoresNuevos,
        HashSet<Categoria> categoriasNuevas,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fila.CodigoInternoRecurso))
            throw new ArgumentException("El c\u00F3digo interno del recurso es obligatorio.");

        if (string.IsNullOrWhiteSpace(fila.Titulo))
            throw new ArgumentException("El t\u00EDtulo es obligatorio.");

        var codigoRecurso = fila.CodigoInternoRecurso.Trim();
        if (codigosRecursos.Contains(codigoRecurso))
            throw new InvalidOperationException("El recurso ya existe en otra fila del archivo.");

        var recursoExistente = await _recursoRepository.ObtenerPorCodigoInternoAsync(
            codigoRecurso,
            cancellationToken);

        if (recursoExistente is not null)
            throw new InvalidOperationException("Ya existe un recurso con el mismo c\u00F3digo interno.");

        var isbn = string.IsNullOrWhiteSpace(fila.Isbn) ? null : fila.Isbn.Trim();
        if (isbn is not null)
        {
            if (isbns.Contains(isbn))
                throw new InvalidOperationException("El ISBN ya existe en otra fila del archivo.");

            var recursoPorIsbn = await _recursoRepository.ObtenerPorIsbnAsync(isbn, cancellationToken);
            if (recursoPorIsbn is not null)
                throw new InvalidOperationException("Ya existe un recurso con el mismo ISBN.");
        }

        var codigoEjemplar = string.IsNullOrWhiteSpace(fila.CodigoInternoEjemplar)
            ? null
            : fila.CodigoInternoEjemplar.Trim();

        if (codigoEjemplar is not null)
        {
            if (codigosEjemplares.Contains(codigoEjemplar))
                throw new InvalidOperationException("El ejemplar ya existe en otra fila del archivo.");

            var ejemplarExistente = await _ejemplarRepository.ObtenerPorCodigoInternoAsync(
                codigoEjemplar,
                cancellationToken);

            if (ejemplarExistente is not null)
                throw new InvalidOperationException("Ya existe un ejemplar con el mismo c\u00F3digo interno.");
        }

        var recurso = new RecursoBibliografico(
            codigoRecurso,
            fila.Titulo,
            isbn,
            fila.Editorial,
            fila.AnioPublicacion,
            fila.Edicion);

        if (codigoEjemplar is not null)
            recurso.AgregarEjemplar(new Ejemplar(recurso, codigoEjemplar, fila.EstadoFisico));

        var autoresNuevosFila = new HashSet<Autor>();
        var categoriasNuevasFila = new HashSet<Categoria>();
        var autoresFila = await ObtenerAutoresAsync(
            fila.Autores,
            autores,
            autoresNuevosFila,
            cancellationToken);
        var categoriasFila = await ObtenerCategoriasAsync(
            fila.Categorias,
            categorias,
            categoriasNuevasFila,
            cancellationToken);

        autoresNuevos.UnionWith(autoresNuevosFila);
        categoriasNuevas.UnionWith(categoriasNuevasFila);

        return new FilaPreparada(
            recurso,
            codigoEjemplar,
            autoresFila,
            categoriasFila,
            "Fila importada correctamente.");
    }

    private async Task<IReadOnlyList<Autor>> ObtenerAutoresAsync(
        IReadOnlyList<string> nombres,
        Dictionary<string, Autor> cache,
        HashSet<Autor> autoresNuevos,
        CancellationToken cancellationToken)
    {
        var resultado = new List<Autor>();
        foreach (var nombre in nombres.Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                     .Select(nombre => nombre.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!cache.TryGetValue(nombre, out var autor))
            {
                autor = await _autorRepository.ObtenerPorNombreAsync(nombre, cancellationToken);
                if (autor is null)
                {
                    autor = new Autor(nombre);
                    autoresNuevos.Add(autor);
                }

                cache[nombre] = autor;
            }

            if (autor.Id <= 0)
                autoresNuevos.Add(autor);

            resultado.Add(autor);
        }

        return resultado;
    }

    private async Task<IReadOnlyList<Categoria>> ObtenerCategoriasAsync(
        IReadOnlyList<string> nombres,
        Dictionary<string, Categoria> cache,
        HashSet<Categoria> categoriasNuevas,
        CancellationToken cancellationToken)
    {
        var resultado = new List<Categoria>();
        foreach (var nombre in nombres.Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                     .Select(nombre => nombre.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!cache.TryGetValue(nombre, out var categoria))
            {
                categoria = await _categoriaRepository.ObtenerPorNombreAsync(nombre, cancellationToken);
                if (categoria is null)
                {
                    categoria = new Categoria(nombre);
                    categoriasNuevas.Add(categoria);
                }

                cache[nombre] = categoria;
            }

            if (categoria.Id <= 0)
                categoriasNuevas.Add(categoria);

            resultado.Add(categoria);
        }

        return resultado;
    }

    private async Task GuardarImportacionAsync(
        ImportarRecursosBibliograficosCommand command,
        int totalFilas,
        IReadOnlyList<FilaPreparada> filasPreparadas,
        IReadOnlyCollection<Autor> autoresNuevos,
        IReadOnlyCollection<Categoria> categoriasNuevas,
        int ejemplaresCreados,
        int recursosOmitidos,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var autor in autoresNuevos)
                await _autorRepository.AgregarAsync(autor, cancellationToken);

            foreach (var categoria in categoriasNuevas)
                await _categoriaRepository.AgregarAsync(categoria, cancellationToken);

            foreach (var fila in filasPreparadas)
                await _recursoRepository.AgregarAsync(fila.Recurso, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var fila in filasPreparadas)
            {
                foreach (var autor in fila.Autores)
                    fila.Recurso.AgregarAutor(new RecursoAutor(fila.Recurso.Id, autor.Id));

                foreach (var categoria in fila.Categorias)
                    fila.Recurso.AgregarCategoria(new RecursoCategoria(fila.Recurso.Id, categoria.Id));
            }

            await RegistrarAuditoriaImportacionAsync(
                command,
                totalFilas,
                filasPreparadas.Count,
                ejemplaresCreados,
                autoresNuevos.Count,
                categoriasNuevas.Count,
                recursosOmitidos,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private Task RegistrarAuditoriaImportacionAsync(
        ImportarRecursosBibliograficosCommand command,
        int totalFilas,
        int recursosCreados,
        int ejemplaresCreados,
        int autoresCreados,
        int categoriasCreadas,
        int recursosOmitidos,
        CancellationToken cancellationToken)
    {
        var detalle = CrearDetalleAuditoria(command.NombreArchivo, totalFilas, recursosCreados,
            ejemplaresCreados, autoresCreados, categoriasCreadas, recursosOmitidos);

        return RegistrarAuditoriaAsync(command.UsuarioResponsableId, detalle, cancellationToken);
    }

    private Task RegistrarAuditoriaAsync(
        int usuarioId,
        string detalle,
        CancellationToken cancellationToken)
    {
        return _auditoriaService.RegistrarAsync(
            usuarioId: usuarioId,
            modulo: "Cat\u00E1logo",
            accion: "Importar recursos bibliogr\u00E1ficos",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: null,
            detalle: detalle,
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);
    }

    private static string CrearDetalleAuditoria(
        string nombreArchivo,
        int totalFilas,
        int recursosCreados,
        int ejemplaresCreados,
        int autoresCreados,
        int categoriasCreadas,
        int recursosOmitidos)
    {
        return $"Archivo: {nombreArchivo.Trim()}. Total filas: {totalFilas}. " +
               $"Recursos creados: {recursosCreados}. Ejemplares creados: {ejemplaresCreados}. " +
               $"Autores creados: {autoresCreados}. Categor\u00EDas creadas: {categoriasCreadas}. " +
               $"Filas omitidas: {recursosOmitidos}.";
    }

    private static DetalleImportacionRecursoBibliograficoDto CrearDetalle(
        FilaImportacionRecursoBibliografico fila,
        bool importada,
        string mensaje) => new()
    {
        NumeroFila = fila.NumeroFila,
        Importada = importada,
        CodigoInternoRecurso = fila.CodigoInternoRecurso?.Trim() ?? string.Empty,
        CodigoInternoEjemplar = string.IsNullOrWhiteSpace(fila.CodigoInternoEjemplar)
            ? null
            : fila.CodigoInternoEjemplar.Trim(),
        Mensaje = mensaje
    };

    private static ApplicationResult ValidarCommand(ImportarRecursosBibliograficosCommand command)
    {
        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");
        if (string.IsNullOrWhiteSpace(command.NombreArchivo))
            return ApplicationResult.Failure("El nombre del archivo es obligatorio.");
        if (string.IsNullOrWhiteSpace(command.ContentType))
            return ApplicationResult.Failure("El tipo de contenido es obligatorio.");
        if (command.TamanoBytes <= 0)
            return ApplicationResult.Failure("El archivo no puede estar vac\u00EDo.");
        if (command.TamanoBytes > TamanoMaximoBytes)
            return ApplicationResult.Failure("El archivo no puede superar los 10 MB.");
        if (command.Contenido == Stream.Null)
            return ApplicationResult.Failure("El contenido del archivo es obligatorio.");
        if (!ExtensionesPermitidas.Contains(Path.GetExtension(command.NombreArchivo)))
            return ApplicationResult.Failure("La extensi\u00F3n del archivo no est\u00E1 permitida.");
        if (!ContentTypesPermitidos.Contains(command.ContentType))
            return ApplicationResult.Failure("El tipo de contenido del archivo no est\u00E1 permitido.");

        return ApplicationResult.Success();
    }

    private sealed record FilaPreparada(
        RecursoBibliografico Recurso,
        string? CodigoInternoEjemplar,
        IReadOnlyList<Autor> Autores,
        IReadOnlyList<Categoria> Categorias,
        string Mensaje);
}
