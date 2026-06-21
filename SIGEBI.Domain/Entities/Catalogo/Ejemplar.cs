using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class Ejemplar : AuditableEntity
{
    public int RecursoBibliograficoId { get; private set; }

    public string CodigoInterno { get; private set; } = string.Empty;

    public string? EstadoFisico { get; private set; }

    public EstadoEjemplar Estado { get; private set; }

    public RecursoBibliografico? RecursoBibliografico { get; private set; }

    public ICollection<HistorialEstadoEjemplar> HistorialEstados { get; private set; } = new List<HistorialEstadoEjemplar>();

    private Ejemplar()
    {
    }

    public Ejemplar(
        int recursoBibliograficoId,
        string codigoInterno,
        string? estadoFisico = null)
    {
        if (recursoBibliograficoId <= 0)
            throw new ArgumentException("El recurso bibliográfico es obligatorio.");

        if (string.IsNullOrWhiteSpace(codigoInterno))
            throw new ArgumentException("El código interno del ejemplar es obligatorio.");

        RecursoBibliograficoId = recursoBibliograficoId;
        CodigoInterno = codigoInterno.Trim();
        EstadoFisico = estadoFisico?.Trim();
        Estado = EstadoEjemplar.Disponible;
    }

    public OperationResult Reservar()
    {
        if (Estado != EstadoEjemplar.Disponible)
            return OperationResult.Failure("El ejemplar no se encuentra disponible.");

        Estado = EstadoEjemplar.Reservado;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult LiberarReserva()
    {
        if (Estado != EstadoEjemplar.Reservado)
            return OperationResult.Failure("El ejemplar no se encuentra reservado.");

        Estado = EstadoEjemplar.Disponible;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult MarcarComoPrestado()
    {
        if (Estado != EstadoEjemplar.Reservado)
            return OperationResult.Failure("El ejemplar debe estar reservado antes de formalizar el préstamo.");

        Estado = EstadoEjemplar.Prestado;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult RegistrarDevolucion()
    {
        if (Estado != EstadoEjemplar.Prestado)
            return OperationResult.Failure("El ejemplar no posee un préstamo activo.");

        Estado = EstadoEjemplar.Disponible;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public void MarcarFueraDeServicio(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe indicar el motivo por el cual el ejemplar queda fuera de servicio.");

        Estado = EstadoEjemplar.FueraDeServicio;
        EstadoFisico = motivo.Trim();
        FechaModificacion = DateTime.UtcNow;
    }
}