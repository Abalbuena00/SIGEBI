using SIGEBI.Domain.Entities.Prestamos;

namespace SIGEBI.Domain.Repository;

public interface IReservaTemporalRepository : IBaseRepository<ReservaTemporal>
{
    Task<ReservaTemporal?> ObtenerActivaPorEjemplarAsync(
        int ejemplarId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaTemporal>> ObtenerVencidasAsync(
        DateTime fechaActual,
        CancellationToken cancellationToken = default);
}