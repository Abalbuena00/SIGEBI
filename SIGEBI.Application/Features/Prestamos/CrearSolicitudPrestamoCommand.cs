namespace SIGEBI.Application.Features.Prestamos;

public sealed class CrearSolicitudPrestamoCommand
{
    public int UsuarioId { get; init; }

    public int EjemplarId { get; init; }
}