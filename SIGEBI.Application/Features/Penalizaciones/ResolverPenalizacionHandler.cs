using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Abstractions.Notificaciones;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Penalizaciones;

public sealed class ResolverPenalizacionHandler
{
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly INotificacionService _notificacionService;
    private readonly IUnitOfWork _unitOfWork;

    public ResolverPenalizacionHandler(
        IPenalizacionRepository penalizacionRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        INotificacionService notificacionService,
        IUnitOfWork unitOfWork)
    {
        _penalizacionRepository = penalizacionRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _notificacionService = notificacionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ResolverPenalizacionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var penalizacion = await _penalizacionRepository.ObtenerPorIdAsync(
            command.PenalizacionId,
            cancellationToken);

        if (penalizacion is null)
            return ApplicationResult.Failure("La penalización no fue encontrada.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var usuarioAfectado = await _usuarioRepository.ObtenerPorIdAsync(
            penalizacion.UsuarioId,
            cancellationToken);

        if (usuarioAfectado is null)
            return ApplicationResult.Failure("El usuario afectado por la penalización no fue encontrado.");

        var resultadoResolucion = penalizacion.Resolver(
            command.UsuarioResponsableId,
            command.Motivo);

        if (!resultadoResolucion.IsSuccess)
            return ApplicationResult.Failure(resultadoResolucion.Error!);

        await _notificacionService.CrearAsync(
            usuarioDestinatarioId: penalizacion.UsuarioId,
            tipo: TipoNotificacion.PenalizacionResuelta,
            titulo: "Penalización resuelta",
            mensaje:
                $"La penalización {penalizacion.Id} fue resuelta. " +
                $"Motivo: {penalizacion.MotivoResolucion}.",
            entidadReferencia: "Penalizacion",
            entidadReferenciaId: penalizacion.Id,
            cancellationToken: cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Penalizaciones",
            accion: "Resolver penalización",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Penalizacion",
            entidadAfectadaId: penalizacion.Id,
            detalle:
                $"Se resolvió la penalización {penalizacion.Id}. " +
                $"Usuario afectado: {penalizacion.UsuarioId}. " +
                $"Motivo: {penalizacion.MotivoResolucion}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ResolverPenalizacionCommand command)
    {
        if (command.PenalizacionId <= 0)
            return ApplicationResult.Failure("La penalización es obligatoria.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Motivo))
            return ApplicationResult.Failure("Debe indicar el motivo de la resolución.");

        return ApplicationResult.Success();
    }
}
