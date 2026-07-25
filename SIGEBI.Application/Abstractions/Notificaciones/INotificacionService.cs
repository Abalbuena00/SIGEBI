using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Abstractions.Notificaciones;

public interface INotificacionService
{
    Task CrearAsync(
        int usuarioDestinatarioId,
        TipoNotificacion tipo,
        string titulo,
        string mensaje,
        string? entidadReferencia = null,
        int? entidadReferenciaId = null,
        CancellationToken cancellationToken = default);
}