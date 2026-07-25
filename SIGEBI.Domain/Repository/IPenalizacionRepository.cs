using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Repository;

public interface IPenalizacionRepository : IBaseRepository<Penalizacion>
{
    Task<IReadOnlyList<Penalizacion>> ObtenerPorUsuarioAsync(
        int usuarioId,
        EstadoPenalizacion? estado = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Penalizacion>> ObtenerActivasPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> TienePenalizacionActivaAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
}
