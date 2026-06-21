using SIGEBI.Domain.Entities.Prestamos;

namespace SIGEBI.Domain.Repository;

public interface IPrestamoRepository : IBaseRepository<Prestamo>
{
    Task<IReadOnlyList<Prestamo>> ObtenerActivosPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Prestamo?> ObtenerActivoPorEjemplarAsync(
        int ejemplarId,
        CancellationToken cancellationToken = default);

    Task<int> ContarActivosPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Prestamo>> ObtenerVencidosAsync(
        DateTime fechaActual,
        CancellationToken cancellationToken = default);
}