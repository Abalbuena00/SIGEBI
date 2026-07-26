namespace SIGEBI.Application.DTOs.Configuracion;

public sealed class PoliticaPrestamoDto
{
    public int Id { get; set; }

    public int TipoMiembro { get; set; }

    public string TipoMiembroDescripcion { get; set; } = string.Empty;

    public int MaximoPrestamosActivos { get; set; }

    public int DiasDuracionPrestamo { get; set; }

    public int HorasReservaTemporal { get; set; }

    public int DiasSuspensionPorDiaRetraso { get; set; }

    public bool PermitePrestamoConVencidos { get; set; }

    public bool PenalizaRetraso { get; set; }

    public bool Activo { get; set; }
}
