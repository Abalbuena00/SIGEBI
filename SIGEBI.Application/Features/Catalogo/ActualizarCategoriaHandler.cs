using SIGEBI.Application.Common;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarCategoriaHandler
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarCategoriaHandler(
        ICategoriaRepository categoriaRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarCategoriaCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (command.CategoriaId <= 0)
            return ApplicationResult.Failure("La categoría es obligatoria.");

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult.Failure("El nombre de la categoría es obligatorio.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(
            command.CategoriaId,
            cancellationToken);

        if (categoria is null)
            return ApplicationResult.Failure("La categoría no fue encontrada.");

        var categoriaExistente = await _categoriaRepository.ObtenerPorNombreAsync(
            command.Nombre,
            cancellationToken);

        if (categoriaExistente is not null && categoriaExistente.Id != categoria.Id)
            return ApplicationResult.Failure("Ya existe otra categoría con el mismo nombre.");

        string nombreAnterior = categoria.Nombre;
        string? descripcionAnterior = categoria.Descripcion;

        categoria.ActualizarDatos(
            command.Nombre,
            command.Descripcion);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Actualizar categor\u00EDa",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Categoria",
            entidadAfectadaId: categoria.Id,
            detalle:
                $"Nombre anterior: {nombreAnterior}; nombre nuevo: {categoria.Nombre}. " +
                $"Descripci\u00F3n anterior: {descripcionAnterior ?? "sin descripci\u00F3n"}; " +
                $"descripci\u00F3n nueva: {categoria.Descripcion ?? "sin descripci\u00F3n"}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}
