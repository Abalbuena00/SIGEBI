using SIGEBI.Domain.Entities.Catalogo;

namespace SIGEBI.Domain.Repository;

public interface IAutorRepository : IBaseRepository<Autor>
{
    Task<Autor?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Autor>> BuscarAsync(
        string? nombre,
        CancellationToken cancellationToken = default);
}