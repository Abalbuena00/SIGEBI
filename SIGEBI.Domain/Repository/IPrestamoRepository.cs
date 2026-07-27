using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;

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

    Task<bool> ExistePrestamoAbiertoPorRecursoAsync(
        int recursoBibliograficoId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Prestamo> Items, int TotalItems)> ConsultarHistorialAsync(
        int? usuarioId,
        int? recursoBibliograficoId,
        int? ejemplarId,
        EstadoPrestamo? estado,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
