using SIGEBI.Domain.Entities.Penalizaciones;

namespace SIGEBI.Domain.Repository;

public interface IPenalizacionRepository : IBaseRepository<Penalizacion>
{
    Task<IReadOnlyList<Penalizacion>> ObtenerActivasPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> TienePenalizacionActivaAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
}