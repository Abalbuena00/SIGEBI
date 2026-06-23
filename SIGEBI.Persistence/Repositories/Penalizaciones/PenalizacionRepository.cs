using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Penalizaciones;

public sealed class PenalizacionRepository
    : BaseRepository<Penalizacion>, IPenalizacionRepository
{
    public PenalizacionRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Obtiene las penalizaciones activas de un usuario.
    public async Task<IReadOnlyList<Penalizacion>> ObtenerActivasPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(penalizacion =>
                penalizacion.Activo &&
                penalizacion.UsuarioId == usuarioId &&
                penalizacion.Estado == EstadoPenalizacion.Activa &&
                penalizacion.FechaFin >= DateTime.UtcNow)
            .OrderBy(penalizacion => penalizacion.FechaFin)
            .ToListAsync(cancellationToken);
    }

    // Verifica si el usuario tiene alguna penalización vigente.
    public async Task<bool> TienePenalizacionActivaAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(
                penalizacion =>
                    penalizacion.Activo &&
                    penalizacion.UsuarioId == usuarioId &&
                    penalizacion.Estado == EstadoPenalizacion.Activa &&
                    penalizacion.FechaFin >= DateTime.UtcNow,
                cancellationToken);
    }
}