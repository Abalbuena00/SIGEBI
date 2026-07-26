namespace SIGEBI.Application.DTOs.Penalizaciones;

public sealed class IncidenciaEjemplarDto
{
    public int Id { get; set; }

    public int EjemplarId { get; set; }

    public int? PrestamoId { get; set; }

    public int UsuarioReportaId { get; set; }

    public int Tipo { get; set; }

    public string TipoDescripcion { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public bool Cerrada { get; set; }

    public DateTime? FechaCierre { get; set; }

    public int? UsuarioCierreId { get; set; }
}
