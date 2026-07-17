using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Catalogo;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarCategoriasHandler
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ConsultarCategoriasHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<CategoriaDto>>> HandleAsync(
        ConsultarCategoriasQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var categorias = await _categoriaRepository.BuscarAsync(
            query.Nombre,
            cancellationToken);

        var resultado = categorias
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<CategoriaDto>>.Success(resultado);
    }

    private static CategoriaDto MapearADto(Categoria categoria)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion
        };
    }
}