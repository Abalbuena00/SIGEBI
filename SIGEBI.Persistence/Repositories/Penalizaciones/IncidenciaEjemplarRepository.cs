using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Penalizaciones;

public sealed class IncidenciaEjemplarRepository
    : BaseRepository<IncidenciaEjemplar>, IIncidenciaEjemplarRepository
{
    public IncidenciaEjemplarRepository(SigebiDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerPorEjemplarAsync(
        int ejemplarId,
        bool? cerrada = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = DbSet
            .AsNoTracking()
            .Where(incidencia =>
                incidencia.Activo &&
                incidencia.EjemplarId == ejemplarId);

        if (cerrada.HasValue)
        {
            consulta = consulta.Where(incidencia =>
                incidencia.Cerrada == cerrada.Value);
        }

        return await consulta
            .OrderByDescending(incidencia => incidencia.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerPorPrestamoAsync(
        int prestamoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(incidencia =>
                incidencia.Activo &&
                incidencia.PrestamoId == prestamoId)
            .OrderByDescending(incidencia => incidencia.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IncidenciaEjemplar>> ObtenerAbiertasAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(incidencia =>
                incidencia.Activo &&
                !incidencia.Cerrada)
            .OrderByDescending(incidencia => incidencia.FechaRegistro)
            .ToListAsync(cancellationToken);
    }
}
