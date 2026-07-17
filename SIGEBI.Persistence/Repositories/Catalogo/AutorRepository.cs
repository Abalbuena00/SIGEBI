using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Catalogo;

public sealed class AutorRepository : BaseRepository<Autor>, IAutorRepository
{
    public AutorRepository(SigebiDbContext context)
        : base(context)
    {
    }

    public async Task<Autor?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        string nombreNormalizado = nombre.Trim().ToLower();

        return await Context.Autores
            .FirstOrDefaultAsync(
                autor => autor.Nombre.ToLower() == nombreNormalizado &&
                         autor.Activo,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Autor>> BuscarAsync(
        string? nombre,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Autores
            .AsNoTracking()
            .Where(autor => autor.Activo);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            string filtro = $"%{nombre.Trim()}%";

            query = query.Where(autor =>
                EF.Functions.Like(autor.Nombre, filtro));
        }

        return await query
            .OrderBy(autor => autor.Nombre)
            .ToListAsync(cancellationToken);
    }
}