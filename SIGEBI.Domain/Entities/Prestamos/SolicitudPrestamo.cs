using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Prestamos;

public sealed class SolicitudPrestamo : AuditableEntity
{
    public int UsuarioId { get; private set; }

    public int EjemplarId { get; private set; }

    public DateTime FechaSolicitud { get; private set; }

    public EstadoSolicitudPrestamo Estado { get; private set; }

    public DateTime FechaExpiracionSolicitud { get; private set; }

    public DateTime? FechaAprobacion { get; private set; }

    public DateTime? FechaRechazo { get; private set; }

    public DateTime? FechaCompletada { get; private set; }

    public int? UsuarioAprobadorId { get; private set; }

    public string? MotivoRechazo { get; private set; }

    private SolicitudPrestamo()
    {
    }

    public SolicitudPrestamo(int usuarioId, int ejemplarId, int horasVigencia = 24)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El usuario solicitante es obligatorio.");

        if (ejemplarId <= 0)
            throw new ArgumentException("El ejemplar es obligatorio.");

        if (horasVigencia <= 0)
            throw new ArgumentException("La vigencia de la solicitud debe ser mayor que cero.");

        UsuarioId = usuarioId;
        EjemplarId = ejemplarId;
        FechaSolicitud = DateTime.UtcNow;
        FechaExpiracionSolicitud = FechaSolicitud.AddHours(horasVigencia);
        Estado = EstadoSolicitudPrestamo.Pendiente;
    }

    public OperationResult Aprobar(int usuarioAprobadorId)
    {
        if (Estado != EstadoSolicitudPrestamo.Pendiente)
            return OperationResult.Failure("Solo se pueden aprobar solicitudes pendientes.");

        if (usuarioAprobadorId <= 0)
            return OperationResult.Failure("El usuario aprobador es obligatorio.");

        Estado = EstadoSolicitudPrestamo.AprobadaPendienteRetiro;
        UsuarioAprobadorId = usuarioAprobadorId;
        FechaAprobacion = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Rechazar(int usuarioAprobadorId, string motivo)
    {
        if (Estado != EstadoSolicitudPrestamo.Pendiente)
            return OperationResult.Failure("Solo se pueden rechazar solicitudes pendientes.");

        if (usuarioAprobadorId <= 0)
            return OperationResult.Failure("El usuario que rechaza es obligatorio.");

        if (string.IsNullOrWhiteSpace(motivo))
            return OperationResult.Failure("Debe indicar el motivo del rechazo.");

        Estado = EstadoSolicitudPrestamo.Rechazada;
        UsuarioAprobadorId = usuarioAprobadorId;
        MotivoRechazo = motivo.Trim();
        FechaRechazo = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Vencer()
    {
        if (Estado != EstadoSolicitudPrestamo.Pendiente &&
            Estado != EstadoSolicitudPrestamo.AprobadaPendienteRetiro)
        {
            return OperationResult.Failure("La solicitud no se encuentra en un estado vencible.");
        }

        Estado = EstadoSolicitudPrestamo.Vencida;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Completar()
    {
        if (Estado != EstadoSolicitudPrestamo.AprobadaPendienteRetiro)
            return OperationResult.Failure("Solo se pueden completar solicitudes aprobadas pendientes de retiro.");

        Estado = EstadoSolicitudPrestamo.Completada;
        FechaCompletada = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Cancelar()
    {
        if (Estado == EstadoSolicitudPrestamo.Completada)
            return OperationResult.Failure("No se puede cancelar una solicitud completada.");

        Estado = EstadoSolicitudPrestamo.Cancelada;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }
}