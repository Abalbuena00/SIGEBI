namespace SIGEBI.Application.Features.Notificaciones;

public sealed class MarcarNotificacionComoLeidaCommand
{
    public int NotificacionId { get; init; }

    public int UsuarioId { get; init; }
}