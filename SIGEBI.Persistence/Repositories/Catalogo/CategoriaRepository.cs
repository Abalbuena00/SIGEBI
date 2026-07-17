using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Catalogo;

public sealed class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(SigebiDbContext context)
        : base(context)
    {
    }

    public async Task<Categoria?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        string nombreNormalizado = nombre.Trim().ToLower();

        return await Context.Categorias
            .FirstOrDefaultAsync(
                categoria => categoria.Nombre.ToLower() == nombreNormalizado &&
                             categoria.Activo,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> BuscarAsync(
        string? nombre,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Categorias
            .AsNoTracking()
            .Where(categoria => categoria.Activo);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            string filtro = $"%{nombre.Trim()}%";

            query = query.Where(categoria =>
                EF.Functions.Like(categoria.Nombre, filtro));
        }

        return await query
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync(cancellationToken);
    }
}