namespace SIGEBI.Application.DTOs.Catalogo;

public sealed class HistorialEstadoEjemplarDto
{
    public int Id { get; set; }
    public int EjemplarId { get; set; }
    public int? EstadoAnterior { get; set; }
    public string? EstadoAnteriorDescripcion { get; set; }
    public int EstadoNuevo { get; set; }
    public string EstadoNuevoDescripcion { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; }
    public int? UsuarioResponsableId { get; set; }
    public string? Motivo { get; set; }
}
