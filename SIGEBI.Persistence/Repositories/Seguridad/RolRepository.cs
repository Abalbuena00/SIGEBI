using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Seguridad;

public sealed class RolRepository : BaseRepository<Rol>, IRolRepository
{
    public RolRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Busca un rol por su nombre lógico: Estudiante, Docente, Administrador, etc.
    public async Task<Rol?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        var nombreNormalizado = nombre.Trim();

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rol => rol.Nombre == nombreNormalizado,
                cancellationToken);
    }
}