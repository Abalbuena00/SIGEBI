using SIGEBI.Domain.Entities.Penalizaciones;

namespace SIGEBI.Domain.Repository;

public interface IIncidenciaEjemplarRepository : IBaseRepository<IncidenciaEjemplar>
{
    Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerPorEjemplarAsync(
        int ejemplarId,
        bool? cerrada = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerPorPrestamoAsync(
        int prestamoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerAbiertasAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerPorRecursoAsync(
        int recursoBibliograficoId,
        CancellationToken cancellationToken = default);
}
