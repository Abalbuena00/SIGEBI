using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Notificaciones;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Notificaciones;

public sealed class NotificacionRepository
    : BaseRepository<Notificacion>, INotificacionRepository
{
    public NotificacionRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Obtiene todas las notificaciones de un usuario, mostrando primero las más recientes.
    public async Task<IReadOnlyList<Notificacion>> ObtenerPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(notificacion =>
                notificacion.Activo &&
                notificacion.UsuarioDestinatarioId == usuarioId)
            .OrderByDescending(notificacion => notificacion.FechaEnvio)
            .ToListAsync(cancellationToken);
    }

    // Obtiene solamente las notificaciones que el usuario todavía no ha leído.
    public async Task<IReadOnlyList<Notificacion>> ObtenerNoLeidasPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(notificacion =>
                notificacion.Activo &&
                notificacion.UsuarioDestinatarioId == usuarioId &&
                notificacion.Estado == EstadoNotificacion.NoLeida)
            .OrderByDescending(notificacion => notificacion.FechaEnvio)
            .ToListAsync(cancellationToken);
    }
}