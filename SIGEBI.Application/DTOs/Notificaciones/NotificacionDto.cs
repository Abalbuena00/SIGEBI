namespace SIGEBI.Application.DTOs.Notificaciones;

public sealed class NotificacionDto
{
    public int Id { get; set; }

    public int UsuarioDestinatarioId { get; set; }

    public int Tipo { get; set; }

    public string TipoDescripcion { get; set; } = string.Empty;

    public int Estado { get; set; }

    public string EstadoDescripcion { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;

    public DateTime FechaEnvio { get; set; }

    public DateTime? FechaLectura { get; set; }

    public string? EntidadReferencia { get; set; }

    public int? EntidadReferenciaId { get; set; }
}