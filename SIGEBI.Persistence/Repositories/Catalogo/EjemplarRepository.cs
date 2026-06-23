using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Catalogo;

public sealed class EjemplarRepository : BaseRepository<Ejemplar>, IEjemplarRepository
{
    public EjemplarRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Busca una copia física específica por su código interno.
    public async Task<Ejemplar?> ObtenerPorCodigoInternoAsync(
        string codigoInterno,
        CancellationToken cancellationToken = default)
    {
        var codigoNormalizado = codigoInterno.Trim();

        return await DbSet
            .AsNoTracking()
            .Include(ejemplar => ejemplar.RecursoBibliografico)
            .FirstOrDefaultAsync(
                ejemplar => ejemplar.CodigoInterno == codigoNormalizado,
                cancellationToken);
    }

    // Obtiene los ejemplares disponibles de un recurso bibliográfico.
    public async Task<IReadOnlyList<Ejemplar>> ObtenerDisponiblesPorRecursoAsync(
        int recursoBibliograficoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(ejemplar =>
                ejemplar.RecursoBibliograficoId == recursoBibliograficoId &&
                ejemplar.Estado == EstadoEjemplar.Disponible &&
                ejemplar.Activo)
            .OrderBy(ejemplar => ejemplar.CodigoInterno)
            .ToListAsync(cancellationToken);
    }

    // Verifica rápidamente si existe al menos un ejemplar disponible.
    public async Task<bool> ExisteEjemplarDisponibleAsync(
        int recursoBibliograficoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(
                ejemplar =>
                    ejemplar.RecursoBibliograficoId == recursoBibliograficoId &&
                    ejemplar.Estado == EstadoEjemplar.Disponible &&
                    ejemplar.Activo,
                cancellationToken);
    }
}