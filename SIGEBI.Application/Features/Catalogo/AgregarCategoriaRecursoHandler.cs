using SIGEBI.Application.Common;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class AgregarCategoriaRecursoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarCategoriaRecursoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        ICategoriaRepository categoriaRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _categoriaRepository = categoriaRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        AgregarCategoriaRecursoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (command.CategoriaId <= 0)
            return ApplicationResult.Failure("La categoría es obligatoria.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var recurso = await _recursoBibliograficoRepository.ObtenerParaActualizarRelacionesAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(
            command.CategoriaId,
            cancellationToken);

        if (categoria is null)
            return ApplicationResult.Failure("La categoría no fue encontrada.");

        if (recurso.Categorias.Any(recursoCategoria => recursoCategoria.CategoriaId == categoria.Id))
            return ApplicationResult.Failure("La categoría ya está asociada a este recurso bibliográfico.");

        var recursoCategoria = new RecursoCategoria(
            recurso.Id,
            categoria.Id);

        recurso.AgregarCategoria(recursoCategoria);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Agregar categor\u00EDa a recurso bibliogr\u00E1fico",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: recurso.Id,
            detalle:
                $"Recurso: {recurso.CodigoInterno} - {recurso.Titulo}. " +
                $"Categor\u00EDa agregada: {categoria.Nombre}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}
