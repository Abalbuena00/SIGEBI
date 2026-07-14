namespace SIGEBI.Application.Features.Prestamos;

public sealed class RegistrarDevolucionPrestamoCommand
{
    public int PrestamoId { get; init; }

    public int UsuarioBibliotecarioId { get; init; }

    public DateTime? FechaDevolucion { get; init; }

    public string? Observacion { get; init; }
}