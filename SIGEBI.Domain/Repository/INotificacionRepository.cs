using SIGEBI.Domain.Entities.Notificaciones;

namespace SIGEBI.Domain.Repository;

public interface INotificacionRepository : IBaseRepository<Notificacion>
{
    Task<IReadOnlyList<Notificacion>> ObtenerPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notificacion>> ObtenerNoLeidasPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
}