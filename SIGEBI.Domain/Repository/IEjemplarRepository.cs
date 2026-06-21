using SIGEBI.Domain.Entities.Catalogo;

namespace SIGEBI.Domain.Repository;

public interface IEjemplarRepository : IBaseRepository<Ejemplar>
{
    Task<Ejemplar?> ObtenerPorCodigoInternoAsync(
        string codigoInterno,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ejemplar>> ObtenerDisponiblesPorRecursoAsync(
        int recursoBibliograficoId,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteEjemplarDisponibleAsync(
        int recursoBibliograficoId,
        CancellationToken cancellationToken = default);
}