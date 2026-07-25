using SIGEBI.Application.Abstractions.Notificaciones;
using SIGEBI.Domain.Entities.Notificaciones;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Services.Notificaciones;

public sealed class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;

    public NotificacionService(INotificacionRepository notificacionRepository)
    {
        _notificacionRepository = notificacionRepository;
    }

    public async Task CrearAsync(
        int usuarioDestinatarioId,
        TipoNotificacion tipo,
        string titulo,
        string mensaje,
        string? entidadReferencia = null,
        int? entidadReferenciaId = null,
        CancellationToken cancellationToken = default)
    {
        var notificacion = new Notificacion(
            usuarioDestinatarioId,
            tipo,
            titulo,
            mensaje,
            entidadReferencia,
            entidadReferenciaId);

        await _notificacionRepository.AgregarAsync(
            notificacion,
            cancellationToken);
    }
}