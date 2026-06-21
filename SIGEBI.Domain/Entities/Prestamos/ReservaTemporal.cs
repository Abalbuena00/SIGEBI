using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Prestamos;

public sealed class ReservaTemporal : AuditableEntity
{
    public int SolicitudPrestamoId { get; private set; }

    public int EjemplarId { get; private set; }

    public DateTime FechaInicio { get; private set; }

    public DateTime FechaExpiracion { get; private set; }

    public EstadoReserva Estado { get; private set; }

    private ReservaTemporal()
    {
    }

    public ReservaTemporal(
        int solicitudPrestamoId,
        int ejemplarId,
        int horasVigencia = 24)
    {
        if (solicitudPrestamoId <= 0)
            throw new ArgumentException("La solicitud de préstamo es obligatoria.");

        if (ejemplarId <= 0)
            throw new ArgumentException("El ejemplar es obligatorio.");

        if (horasVigencia <= 0)
            throw new ArgumentException("La vigencia de la reserva debe ser mayor que cero.");

        SolicitudPrestamoId = solicitudPrestamoId;
        EjemplarId = ejemplarId;
        FechaInicio = DateTime.UtcNow;
        FechaExpiracion = FechaInicio.AddHours(horasVigencia);
        Estado = EstadoReserva.Activa;
    }

    public bool EstaVigente()
    {
        return Estado == EstadoReserva.Activa &&
               DateTime.UtcNow <= FechaExpiracion;
    }

    public OperationResult Vencer()
    {
        if (Estado != EstadoReserva.Activa)
            return OperationResult.Failure("Solo se pueden vencer reservas activas.");

        Estado = EstadoReserva.Vencida;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Cancelar()
    {
        if (Estado != EstadoReserva.Activa)
            return OperationResult.Failure("Solo se pueden cancelar reservas activas.");

        Estado = EstadoReserva.Cancelada;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Utilizar()
    {
        if (!EstaVigente())
            return OperationResult.Failure("La reserva no se encuentra vigente.");

        Estado = EstadoReserva.Utilizada;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }
}