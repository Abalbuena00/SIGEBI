using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Catalogo;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarCatalogoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;

    public ConsultarCatalogoHandler(IRecursoBibliograficoRepository recursoBibliograficoRepository)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<RecursoBibliograficoDto>>> HandleAsync(
        ConsultarCatalogoQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var recursos = await _recursoBibliograficoRepository.BuscarAsync(
            query.Titulo,
            query.Autor,
            query.Categoria,
            query.Disponible,
            cancellationToken);

        var resultado = recursos
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<RecursoBibliograficoDto>>.Success(resultado);
    }

    private static RecursoBibliograficoDto MapearADto(RecursoBibliografico recurso)
    {
        return new RecursoBibliograficoDto
        {
            Id = recurso.Id,
            CodigoInterno = recurso.CodigoInterno,
            Titulo = recurso.Titulo,
            Isbn = recurso.Isbn,
            Editorial = recurso.Editorial,
            AnioPublicacion = recurso.AnioPublicacion,
            Edicion = recurso.Edicion,
            ImagenPortadaUrl = recurso.ImagenPortadaUrl,
            ImagenContraportadaUrl = recurso.ImagenContraportadaUrl,
            CantidadEjemplares = recurso.Ejemplares.Count,
            CantidadEjemplaresDisponibles = recurso.Ejemplares.Count(ejemplar =>
                ejemplar.Estado == EstadoEjemplar.Disponible)
        };
    }
}