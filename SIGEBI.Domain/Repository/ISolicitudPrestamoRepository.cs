using SIGEBI.Domain.Entities.Prestamos;

namespace SIGEBI.Domain.Repository;

public interface ISolicitudPrestamoRepository : IBaseRepository<SolicitudPrestamo>
{
    Task<IReadOnlyList<SolicitudPrestamo>> ObtenerPendientesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SolicitudPrestamo>> ObtenerPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SolicitudPrestamo>> ObtenerVenciblesAsync(
        DateTime fechaActual,
        CancellationToken cancellationToken = default);
}