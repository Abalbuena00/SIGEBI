namespace SIGEBI.Application.DTOs.Penalizaciones;

public sealed class PenalizacionDto
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int? PrestamoId { get; set; }

    public int DiasSuspension { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int Estado { get; set; }

    public string EstadoDescripcion { get; set; } = string.Empty;

    public DateTime? FechaResolucion { get; set; }

    public int? UsuarioResolutorId { get; set; }

    public string? MotivoResolucion { get; set; }
}
