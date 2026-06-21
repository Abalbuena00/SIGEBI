using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Penalizaciones;

public sealed class Penalizacion : AuditableEntity
{
    public int UsuarioId { get; private set; }

    public int? PrestamoId { get; private set; }

    public int DiasSuspension { get; private set; }

    public DateTime FechaInicio { get; private set; }

    public DateTime FechaFin { get; private set; }

    public EstadoPenalizacion Estado { get; private set; }

    public DateTime? FechaResolucion { get; private set; }

    public int? UsuarioResolutorId { get; private set; }

    public string? MotivoResolucion { get; private set; }

    private Penalizacion()
    {
    }

    public Penalizacion(
        int usuarioId,
        int diasSuspension,
        int? prestamoId = null)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El usuario penalizado es obligatorio.");

        if (diasSuspension <= 0)
            throw new ArgumentException("Los días de suspensión deben ser mayores que cero.");

        UsuarioId = usuarioId;
        DiasSuspension = diasSuspension;
        PrestamoId = prestamoId;
        FechaInicio = DateTime.UtcNow;
        FechaFin = FechaInicio.AddDays(diasSuspension);
        Estado = EstadoPenalizacion.Activa;
    }

    public bool EstaActiva()
    {
        return Estado == EstadoPenalizacion.Activa &&
               DateTime.UtcNow.Date <= FechaFin.Date;
    }

    public OperationResult Resolver(int usuarioResolutorId, string motivo)
    {
        if (Estado != EstadoPenalizacion.Activa)
            return OperationResult.Failure("Solo se pueden resolver penalizaciones activas.");

        if (usuarioResolutorId <= 0)
            return OperationResult.Failure("El usuario resolutor es obligatorio.");

        if (string.IsNullOrWhiteSpace(motivo))
            return OperationResult.Failure("Debe indicar el motivo de la resolución.");

        Estado = EstadoPenalizacion.Resuelta;
        UsuarioResolutorId = usuarioResolutorId;
        MotivoResolucion = motivo.Trim();
        FechaResolucion = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult Exonerar(int usuarioResolutorId, string motivo)
    {
        if (Estado != EstadoPenalizacion.Activa)
            return OperationResult.Failure("Solo se pueden exonerar penalizaciones activas.");

        if (usuarioResolutorId <= 0)
            return OperationResult.Failure("El usuario que exonera es obligatorio.");

        if (string.IsNullOrWhiteSpace(motivo))
            return OperationResult.Failure("Debe indicar el motivo de la exoneración.");

        Estado = EstadoPenalizacion.Exonerada;
        UsuarioResolutorId = usuarioResolutorId;
        MotivoResolucion = motivo.Trim();
        FechaResolucion = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }
}