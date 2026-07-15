namespace SIGEBI.Application.Features.Prestamos;

public sealed class CancelarSolicitudPrestamoCommand
{
    public int SolicitudPrestamoId { get; init; }

    public int UsuarioCancelaId { get; init; }
}