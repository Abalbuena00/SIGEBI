using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Notificaciones;

public sealed class Notificacion : AuditableEntity
{
    public int UsuarioDestinatarioId { get; private set; }

    public TipoNotificacion Tipo { get; private set; }

    public EstadoNotificacion Estado { get; private set; }

    public string Titulo { get; private set; } = string.Empty;

    public string Mensaje { get; private set; } = string.Empty;

    public DateTime FechaEnvio { get; private set; }

    public DateTime? FechaLectura { get; private set; }

    public string? EntidadReferencia { get; private set; }

    public int? EntidadReferenciaId { get; private set; }

    private Notificacion()
    {
    }

    public Notificacion(
        int usuarioDestinatarioId,
        TipoNotificacion tipo,
        string titulo,
        string mensaje,
        string? entidadReferencia = null,
        int? entidadReferenciaId = null)
    {
        if (usuarioDestinatarioId <= 0)
            throw new ArgumentException("El usuario destinatario es obligatorio.");

        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título de la notificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(mensaje))
            throw new ArgumentException("El mensaje de la notificación es obligatorio.");

        UsuarioDestinatarioId = usuarioDestinatarioId;
        Tipo = tipo;
        Titulo = titulo.Trim();
        Mensaje = mensaje.Trim();
        EntidadReferencia = entidadReferencia?.Trim();
        EntidadReferenciaId = entidadReferenciaId;
        Estado = EstadoNotificacion.NoLeida;
        FechaEnvio = DateTime.UtcNow;
    }

    public OperationResult MarcarComoLeida()
    {
        if (Estado == EstadoNotificacion.Leida)
            return OperationResult.Failure("La notificación ya fue marcada como leída.");

        Estado = EstadoNotificacion.Leida;
        FechaLectura = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }
}