namespace SIGEBI.Application.Features.Prestamos;

public sealed class GenerarRecordatoriosPrestamosPorVencerCommand
{
    public DateTime? FechaReferencia { get; init; }
    public int DiasAnticipacion { get; init; } = 1;
}
