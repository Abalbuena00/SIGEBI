using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Prestamos;

public sealed class Prestamo : AuditableEntity
{
    public int UsuarioId { get; private set; }

    public int EjemplarId { get; private set; }

    public int? SolicitudPrestamoId { get; private set; }

    public int UsuarioBibliotecarioId { get; private set; }

    public DateTime FechaInicio { get; private set; }

    public DateTime FechaLimiteDevolucion { get; private set; }

    public DateTime? FechaDevolucion { get; private set; }

    public EstadoPrestamo Estado { get; private set; }

    public ICollection<Devolucion> Devoluciones { get; private set; } = new List<Devolucion>();

    private Prestamo()
    {
    }

    public Prestamo(
        int usuarioId,
        int ejemplarId,
        int usuarioBibliotecarioId,
        DateTime fechaLimiteDevolucion,
        int? solicitudPrestamoId = null)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El usuario es obligatorio.");

        if (ejemplarId <= 0)
            throw new ArgumentException("El ejemplar es obligatorio.");

        if (usuarioBibliotecarioId <= 0)
            throw new ArgumentException("El bibliotecario responsable es obligatorio.");

        FechaInicio = DateTime.UtcNow;

        if (fechaLimiteDevolucion <= FechaInicio)
            throw new ArgumentException("La fecha límite de devolución debe ser posterior a la fecha de inicio.");

        UsuarioId = usuarioId;
        EjemplarId = ejemplarId;
        UsuarioBibliotecarioId = usuarioBibliotecarioId;
        FechaLimiteDevolucion = fechaLimiteDevolucion;
        SolicitudPrestamoId = solicitudPrestamoId;
        Estado = EstadoPrestamo.Activo;
    }

    public bool EstaVencido()
    {
        return Estado == EstadoPrestamo.Activo &&
               DateTime.UtcNow.Date > FechaLimiteDevolucion.Date;
    }

    public OperationResult MarcarComoVencido()
    {
        if (Estado != EstadoPrestamo.Activo)
            return OperationResult.Failure("Solo se pueden marcar como vencidos préstamos activos.");

        if (!EstaVencido())
            return OperationResult.Failure("El préstamo aún no ha vencido.");

        Estado = EstadoPrestamo.Vencido;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult RegistrarDevolucion(DateTime fechaDevolucion)
    {
        if (Estado != EstadoPrestamo.Activo && Estado != EstadoPrestamo.Vencido)
            return OperationResult.Failure("El préstamo no se encuentra activo.");

        FechaDevolucion = fechaDevolucion;
        Estado = EstadoPrestamo.Devuelto;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }

    public OperationResult CerrarConIncidencia(DateTime fechaCierre)
    {
        if (Estado != EstadoPrestamo.Activo && Estado != EstadoPrestamo.Vencido)
            return OperationResult.Failure("El préstamo no se encuentra activo.");

        FechaDevolucion = fechaCierre;
        Estado = EstadoPrestamo.CerradoConIncidencia;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }
}