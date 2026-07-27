namespace SIGEBI.Application.DTOs.Prestamos;

public sealed class ResultadoRecordatoriosPrestamosDto
{
    public int PrestamosEvaluados { get; init; }
    public int NotificacionesCreadas { get; init; }
    public DateTime FechaReferencia { get; init; }
    public DateTime FechaLimiteEvaluada { get; init; }
}
