using SIGEBI.Domain.Entities.Catalogo;

namespace SIGEBI.Domain.Repository;

public interface ICategoriaRepository : IBaseRepository<Categoria>
{
    Task<Categoria?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Categoria>> BuscarAsync(
        string? nombre,
        CancellationToken cancellationToken = default);
}