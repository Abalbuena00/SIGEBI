namespace SIGEBI.Application.DTOs.Prestamos;

public sealed class SolicitudPrestamoDto
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int EjemplarId { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public DateTime FechaExpiracionSolicitud { get; set; }

    public int Estado { get; set; }

    public string EstadoDescripcion { get; set; } = string.Empty;

    public DateTime? FechaAprobacion { get; set; }

    public DateTime? FechaRechazo { get; set; }

    public DateTime? FechaCompletada { get; set; }

    public int? UsuarioAprobadorId { get; set; }

    public string? MotivoRechazo { get; set; }
}