using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;
using SIGEBI.Domain.Enums;

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

    // Busca un recurso por ID y devuelve el detalle completo incluyendo autores, categorías y ejemplares.
    public async Task<RecursoBibliografico?> ObtenerDetallePorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await Context.RecursosBibliograficos
            .AsNoTracking()
            .Include(recurso => recurso.Ejemplares)
            .Include(recurso => recurso.Autores)
                .ThenInclude(recursoAutor => recursoAutor.Autor)
            .Include(recurso => recurso.Categorias)
                .ThenInclude(recursoCategoria => recursoCategoria.Categoria)
            .FirstOrDefaultAsync(
                recurso => recurso.Id == id && recurso.Activo,
                cancellationToken);
    }

    // ObtenerParaActualizarRelacionesAsync busca un recurso por ID y devuelve el detalle completo incluyendo autores y categorías, pero no incluye los ejemplares.
    // Esto es útil cuando se desea actualizar las relaciones del recurso sin necesidad de cargar los ejemplares.
    public async Task<RecursoBibliografico?> ObtenerParaActualizarRelacionesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await Context.RecursosBibliograficos
            .Include(recurso => recurso.Autores)
            .Include(recurso => recurso.Categorias)
            .FirstOrDefaultAsync(
                recurso => recurso.Id == id && recurso.Activo,
                cancellationToken);
    }

    // Remueve un autor de un recurso bibliográfico.
    public void RemoverAutor(RecursoAutor recursoAutor)
    {
        Context.RecursosAutores.Remove(recursoAutor);
    }

    // Remueve una categoría de un recurso bibliográfico.
    public void RemoverCategoria(RecursoCategoria recursoCategoria)
    {
        Context.RecursosCategorias.Remove(recursoCategoria);
    }

    // Permite consultar el catálogo por título, autor o categoría.
    public async Task<(IReadOnlyList<RecursoBibliografico> Items, int TotalCount)> BuscarAsync(
        string? titulo,
        string? autor,
        string? categoria,
        bool? disponible,
        int pageNumber,
        int pageSize,
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

        if (disponible.HasValue)
        {
            if (disponible.Value)
            {
                query = query.Where(recurso =>
                    recurso.Ejemplares.Any(ejemplar =>
                        ejemplar.Estado == EstadoEjemplar.Disponible));
            }
            else
            {
                query = query.Where(recurso =>
                    !recurso.Ejemplares.Any(ejemplar =>
                        ejemplar.Estado == EstadoEjemplar.Disponible));
            }
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(recurso => recurso.Titulo)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
