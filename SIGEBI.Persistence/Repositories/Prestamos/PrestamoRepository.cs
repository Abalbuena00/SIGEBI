using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Prestamos;

public sealed class PrestamoRepository : BaseRepository<Prestamo>, IPrestamoRepository
{
    public PrestamoRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Obtiene los préstamos activos de un usuario para validar límites de préstamo.
    public async Task<IReadOnlyList<Prestamo>> ObtenerActivosPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(prestamo =>
                prestamo.Activo &&
                prestamo.UsuarioId == usuarioId &&
                prestamo.Estado == EstadoPrestamo.Activo)
            .OrderBy(prestamo => prestamo.FechaLimiteDevolucion)
            .ToListAsync(cancellationToken);
    }

    // Verifica si un ejemplar ya tiene un préstamo activo.
    public async Task<Prestamo?> ObtenerActivoPorEjemplarAsync(
        int ejemplarId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                prestamo =>
                    prestamo.Activo &&
                    prestamo.EjemplarId == ejemplarId &&
                    prestamo.Estado == EstadoPrestamo.Activo,
                cancellationToken);
    }

    // Cuenta préstamos activos para comparar contra la política de préstamo.
    public async Task<int> ContarActivosPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(
                prestamo =>
                    prestamo.Activo &&
                    prestamo.UsuarioId == usuarioId &&
                    prestamo.Estado == EstadoPrestamo.Activo,
                cancellationToken);
    }

    // Obtiene préstamos activos cuya fecha límite ya pasó.
    public async Task<IReadOnlyList<Prestamo>> ObtenerVencidosAsync(
        DateTime fechaActual,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(prestamo =>
                prestamo.Activo &&
                prestamo.Estado == EstadoPrestamo.Activo &&
                prestamo.FechaLimiteDevolucion < fechaActual)
            .OrderBy(prestamo => prestamo.FechaLimiteDevolucion)
            .ToListAsync(cancellationToken);
    }
}