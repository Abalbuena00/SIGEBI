namespace SIGEBI.Application.Features.Prestamos;

public sealed class AprobarSolicitudPrestamoCommand
{
    public int SolicitudPrestamoId { get; init; }

    public int UsuarioAprobadorId { get; init; }
}