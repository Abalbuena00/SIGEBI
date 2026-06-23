using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Configuracion;

public sealed class ParametroSistemaRepository
    : BaseRepository<ParametroSistema>, IParametroSistemaRepository
{
    public ParametroSistemaRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Busca un parámetro por su clave lógica.
    public async Task<ParametroSistema?> ObtenerPorClaveAsync(
        string clave,
        CancellationToken cancellationToken = default)
    {
        var claveNormalizada = clave.Trim();

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                parametro =>
                    parametro.Activo &&
                    parametro.Clave == claveNormalizada,
                cancellationToken);
    }
}