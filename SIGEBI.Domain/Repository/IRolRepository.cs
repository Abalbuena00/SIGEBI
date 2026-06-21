using SIGEBI.Domain.Entities.Seguridad;

namespace SIGEBI.Domain.Repository;

public interface IRolRepository : IBaseRepository<Rol>
{
    Task<Rol?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);
}