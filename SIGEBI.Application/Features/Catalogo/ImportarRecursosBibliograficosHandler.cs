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
        var autores = new Dictionary<string, Autor?>(StringComparer.OrdinalIgnoreCase);
        var categorias = new Dictionary<string, Categoria?>(StringComparer.OrdinalIgnoreCase);

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
                    cancellationToken);

                await _recursoRepository.AgregarAsync(preparada.Recurso, cancellationToken);
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
        var relacionesAutores = filasPreparadas.Sum(fila => fila.RelacionesAutores);
        var relacionesCategorias = filasPreparadas.Sum(fila => fila.RelacionesCategorias);

        if (recursosCreados > 0)
        {
            await _auditoriaService.RegistrarAsync(
                usuarioId: command.UsuarioResponsableId,
                modulo: "Cat\u00E1logo",
                accion: "Importar recursos bibliogr\u00E1ficos",
                resultado: ResultadoAuditoria.Exitoso,
                entidadAfectada: "RecursoBibliografico",
                entidadAfectadaId: null,
                detalle:
                    $"Archivo: {command.NombreArchivo.Trim()}. Total filas: {filas.Count}. " +
                    $"Recursos creados: {recursosCreados}. Ejemplares creados: {ejemplaresCreados}. " +
                    "Autores creados: 0. Categor\u00EDas creadas: 0. " +
                    $"Filas omitidas: {recursosOmitidos}.",
                origen: "Aplicaci\u00F3n institucional",
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }

        var resultado = new ResultadoImportacionRecursosBibliograficosDto
        {
            TotalFilas = filas.Count,
            RecursosCreados = recursosCreados,
            RecursosOmitidos = recursosOmitidos,
            EjemplaresCreados = ejemplaresCreados,
            AutoresCreados = 0,
            CategoriasCreadas = 0,
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
        Dictionary<string, Autor?> autores,
        Dictionary<string, Categoria?> categorias,
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

        var autoresFila = await ObtenerAutoresAsync(fila.Autores, autores, cancellationToken);
        var categoriasFila = await ObtenerCategoriasAsync(fila.Categorias, categorias, cancellationToken);

        if (codigoEjemplar is not null)
            recurso.AgregarEjemplar(new Ejemplar(recurso, codigoEjemplar, fila.EstadoFisico));

        foreach (var autor in autoresFila.Existentes)
            recurso.AgregarAutor(new RecursoAutor(recurso, autor));

        foreach (var categoria in categoriasFila.Existentes)
            recurso.AgregarCategoria(new RecursoCategoria(recurso, categoria));

        return new FilaPreparada(
            recurso,
            codigoEjemplar,
            autoresFila.Existentes.Count,
            categoriasFila.Existentes.Count,
            CrearMensajeImportacion(autoresFila.Omitidos, categoriasFila.Omitidos));
    }

    private async Task<RelacionesEncontradas<Autor>> ObtenerAutoresAsync(
        IReadOnlyList<string> nombres,
        Dictionary<string, Autor?> cache,
        CancellationToken cancellationToken)
    {
        var resultado = new List<Autor>();
        var omitidos = new List<string>();
        foreach (var nombre in nombres.Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                     .Select(nombre => nombre.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!cache.TryGetValue(nombre, out var autor))
            {
                autor = await _autorRepository.ObtenerPorNombreAsync(nombre, cancellationToken);
                cache[nombre] = autor;
            }

            if (autor is null)
                omitidos.Add(nombre);
            else
                resultado.Add(autor);
        }

        return new RelacionesEncontradas<Autor>(resultado, omitidos);
    }

    private async Task<RelacionesEncontradas<Categoria>> ObtenerCategoriasAsync(
        IReadOnlyList<string> nombres,
        Dictionary<string, Categoria?> cache,
        CancellationToken cancellationToken)
    {
        var resultado = new List<Categoria>();
        var omitidos = new List<string>();
        foreach (var nombre in nombres.Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                     .Select(nombre => nombre.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!cache.TryGetValue(nombre, out var categoria))
            {
                categoria = await _categoriaRepository.ObtenerPorNombreAsync(nombre, cancellationToken);
                cache[nombre] = categoria;
            }

            if (categoria is null)
                omitidos.Add(nombre);
            else
                resultado.Add(categoria);
        }

        return new RelacionesEncontradas<Categoria>(resultado, omitidos);
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


    private static string CrearMensajeImportacion(
        IReadOnlyList<string> autoresOmitidos,
        IReadOnlyList<string> categoriasOmitidas)
    {
        var mensajes = new List<string> { "Fila importada correctamente." };

        if (autoresOmitidos.Count > 0)
            mensajes.Add($"Autores no asociados porque no existen: {string.Join(", ", autoresOmitidos)}.");

        if (categoriasOmitidas.Count > 0)
            mensajes.Add($"Categor\u00EDas no asociadas porque no existen: {string.Join(", ", categoriasOmitidas)}.");

        return string.Join(" ", mensajes);
    }


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
        int RelacionesAutores,
        int RelacionesCategorias,
        string Mensaje);

    private sealed record RelacionesEncontradas<T>(
        IReadOnlyList<T> Existentes,
        IReadOnlyList<string> Omitidos);
}
