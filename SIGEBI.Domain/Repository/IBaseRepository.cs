using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Repository;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        T entity,
        CancellationToken cancellationToken = default);

    void Actualizar(T entity);

    void Remover(T entity);
}