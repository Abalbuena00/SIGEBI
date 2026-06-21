using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Prestamos;

public sealed class Devolucion : BaseEntity
{
    public int PrestamoId { get; private set; }

    public int UsuarioBibliotecarioId { get; private set; }

    public DateTime FechaDevolucion { get; private set; }

    public bool FueTardia { get; private set; }

    public int DiasRetraso { get; private set; }

    public string? Observacion { get; private set; }

    public Prestamo? Prestamo { get; private set; }

    private Devolucion()
    {
    }

    public Devolucion(
        int prestamoId,
        int usuarioBibliotecarioId,
        DateTime fechaLimiteDevolucion,
        DateTime? fechaDevolucion = null,
        string? observacion = null)
    {
        if (prestamoId <= 0)
            throw new ArgumentException("El préstamo es obligatorio.");

        if (usuarioBibliotecarioId <= 0)
            throw new ArgumentException("El bibliotecario responsable es obligatorio.");

        PrestamoId = prestamoId;
        UsuarioBibliotecarioId = usuarioBibliotecarioId;
        FechaDevolucion = fechaDevolucion ?? DateTime.UtcNow;
        Observacion = observacion?.Trim();

        CalcularRetraso(fechaLimiteDevolucion);
    }

    private void CalcularRetraso(DateTime fechaLimiteDevolucion)
    {
        if (FechaDevolucion.Date <= fechaLimiteDevolucion.Date)
        {
            FueTardia = false;
            DiasRetraso = 0;
            return;
        }

        FueTardia = true;
        DiasRetraso = (FechaDevolucion.Date - fechaLimiteDevolucion.Date).Days;
    }
}