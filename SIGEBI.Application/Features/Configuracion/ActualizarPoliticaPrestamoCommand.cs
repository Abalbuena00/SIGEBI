namespace SIGEBI.Application.Features.Configuracion;

public sealed class ActualizarPoliticaPrestamoCommand
{
    public int PoliticaPrestamoId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public int MaximoPrestamosActivos { get; init; }

    public int DiasDuracionPrestamo { get; init; }

    public int HorasReservaTemporal { get; init; }

    public int DiasSuspensionPorDiaRetraso { get; init; }

    public bool PermitePrestamoConVencidos { get; init; }

    public bool PenalizaRetraso { get; init; }
}
