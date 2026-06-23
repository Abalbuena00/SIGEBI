using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Configuracion;

public sealed class PoliticaPrestamoRepository
    : BaseRepository<PoliticaPrestamo>, IPoliticaPrestamoRepository
{
    public PoliticaPrestamoRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Obtiene la política aplicable según el tipo de miembro: Estudiante o Docente.
    public async Task<PoliticaPrestamo?> ObtenerPorTipoMiembroAsync(
        TipoMiembro tipoMiembro,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                politica =>
                    politica.Activo &&
                    politica.TipoMiembro == tipoMiembro,
                cancellationToken);
    }
}