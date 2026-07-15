namespace SIGEBI.Application.Features.Prestamos;

public sealed class RechazarSolicitudPrestamoCommand
{
    public int SolicitudPrestamoId { get; init; }

    public int UsuarioRechazaId { get; init; }

    public string Motivo { get; init; } = string.Empty;
}