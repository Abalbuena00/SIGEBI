using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Catalogo;

public sealed class RecursoBibliograficoRepository
    : BaseRepository<RecursoBibliografico>, IRecursoBibliograficoRepository
{
    public RecursoBibliograficoRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Busca un recurso por su código interno institucional.
    public async Task<RecursoBibliografico?> ObtenerPorCodigoInternoAsync(
        string codigoInterno,
        CancellationToken cancellationToken = default)
    {
        var codigoNormalizado = codigoInterno.Trim();

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                recurso => recurso.CodigoInterno == codigoNormalizado,
                cancellationToken);
    }

    // Busca un recurso por ISBN cuando el registro bibliográfico lo tenga.
    public async Task<RecursoBibliografico?> ObtenerPorIsbnAsync(
        string isbn,
        CancellationToken cancellationToken = default)
    {
        var isbnNormalizado = isbn.Trim();

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                recurso => recurso.Isbn == isbnNormalizado,
                cancellationToken);
    }

    // Permite consultar el catálogo por título, autor o categoría.
    public async Task<IReadOnlyList<RecursoBibliografico>> BuscarAsync(
        string? titulo,
        string? autor,
        string? categoria,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(recurso => recurso.Autores)
                .ThenInclude(recursoAutor => recursoAutor.Autor)
            .Include(recurso => recurso.Categorias)
                .ThenInclude(recursoCategoria => recursoCategoria.Categoria)
            .Include(recurso => recurso.Ejemplares)
            .Where(recurso => recurso.Activo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(titulo))
        {
            var tituloNormalizado = titulo.Trim();

            query = query.Where(recurso =>
                EF.Functions.Like(recurso.Titulo, $"%{tituloNormalizado}%"));
        }

        if (!string.IsNullOrWhiteSpace(autor))
        {
            var autorNormalizado = autor.Trim();

            query = query.Where(recurso =>
                recurso.Autores.Any(recursoAutor =>
                    EF.Functions.Like(recursoAutor.Autor!.Nombre, $"%{autorNormalizado}%")));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            var categoriaNormalizada = categoria.Trim();

            query = query.Where(recurso =>
                recurso.Categorias.Any(recursoCategoria =>
                    EF.Functions.Like(recursoCategoria.Categoria!.Nombre, $"%{categoriaNormalizada}%")));
        }

        return await query
            .OrderBy(recurso => recurso.Titulo)
            .ToListAsync(cancellationToken);
    }
}