namespace SIGEBI.Application.Features.Prestamos;

public sealed class FormalizarPrestamoCommand
{
    public int SolicitudPrestamoId { get; init; }

    public int UsuarioBibliotecarioId { get; init; }
}