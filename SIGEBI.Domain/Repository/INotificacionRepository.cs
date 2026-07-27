using SIGEBI.Domain.Entities.Notificaciones;

using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Repository;

public interface INotificacionRepository : IBaseRepository<Notificacion>
{
    Task<IReadOnlyList<Notificacion>> ObtenerPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notificacion>> ObtenerNoLeidasPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistePorReferenciaAsync(
        int usuarioDestinatarioId,
        TipoNotificacion tipo,
        string entidadReferencia,
        int entidadReferenciaId,
        CancellationToken cancellationToken = default);
}
