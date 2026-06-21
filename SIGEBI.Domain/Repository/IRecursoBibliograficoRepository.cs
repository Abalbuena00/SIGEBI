using SIGEBI.Domain.Entities.Catalogo;

namespace SIGEBI.Domain.Repository;

public interface IRecursoBibliograficoRepository : IBaseRepository<RecursoBibliografico>
{
    Task<RecursoBibliografico?> ObtenerPorCodigoInternoAsync(
        string codigoInterno,
        CancellationToken cancellationToken = default);

    Task<RecursoBibliografico?> ObtenerPorIsbnAsync(
        string isbn,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecursoBibliografico>> BuscarAsync(
        string? titulo,
        string? autor,
        string? categoria,
        CancellationToken cancellationToken = default);
}