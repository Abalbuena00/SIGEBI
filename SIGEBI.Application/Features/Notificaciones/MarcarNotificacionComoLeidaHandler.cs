using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Notificaciones;

public sealed class MarcarNotificacionComoLeidaHandler
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarNotificacionComoLeidaHandler(
        INotificacionRepository notificacionRepository,
        IUnitOfWork unitOfWork)
    {
        _notificacionRepository = notificacionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        MarcarNotificacionComoLeidaCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.NotificacionId <= 0)
            return ApplicationResult.Failure("La notificación es obligatoria.");

        if (command.UsuarioId <= 0)
            return ApplicationResult.Failure("El usuario es obligatorio.");

        var notificacion = await _notificacionRepository.ObtenerPorIdAsync(
            command.NotificacionId,
            cancellationToken);

        if (notificacion is null)
            return ApplicationResult.Failure("La notificación no fue encontrada.");

        if (notificacion.UsuarioDestinatarioId != command.UsuarioId)
            return ApplicationResult.Failure("La notificación no pertenece al usuario indicado.");

        var resultado = notificacion.MarcarComoLeida();

        if (!resultado.IsSuccess)
            return ApplicationResult.Failure(resultado.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}