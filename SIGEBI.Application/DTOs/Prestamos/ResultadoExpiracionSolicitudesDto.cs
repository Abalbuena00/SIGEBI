namespace SIGEBI.Application.DTOs.Prestamos;

public sealed class ResultadoExpiracionSolicitudesDto
{
    public int SolicitudesPendientesVencidas { get; set; }

    public int SolicitudesAprobadasVencidas { get; set; }

    public int ReservasTemporalesVencidas { get; set; }

    public int EjemplaresLiberados { get; set; }
}