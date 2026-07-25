namespace SIGEBI.Application.Features.Notificaciones;

public sealed class ConsultarNotificacionesUsuarioQuery
{
    public int UsuarioId { get; init; }

    public bool SoloNoLeidas { get; init; }
}