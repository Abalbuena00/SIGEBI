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

    public async Task<ApplicationResult<PagedResult<RecursoBibliograficoDto>>> HandleAsync(
        ConsultarCatalogoQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var validacion = ValidarPaginacion(
            query.PageNumber,
            query.PageSize);

        if (!validacion.IsSuccess)
            return ApplicationResult<PagedResult<RecursoBibliograficoDto>>.Failure(validacion.Error!);

        var resultadoConsulta = await _recursoBibliograficoRepository.BuscarAsync(
            query.Titulo,
            query.Autor,
            query.Categoria,
            query.Disponible,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var items = resultadoConsulta.Items
            .Select(MapearADto)
            .ToList();

        var resultado = new PagedResult<RecursoBibliograficoDto>(
            items,
            resultadoConsulta.TotalCount,
            query.PageNumber,
            query.PageSize);

        return ApplicationResult<PagedResult<RecursoBibliograficoDto>>.Success(resultado);
    }

    private static ApplicationResult ValidarPaginacion(
    int pageNumber,
    int pageSize)
    {
        if (pageNumber <= 0)
            return ApplicationResult.Failure("El número de página debe ser mayor que cero.");

        if (pageSize <= 0)
            return ApplicationResult.Failure("El tamaño de página debe ser mayor que cero.");

        if (pageSize > 100)
            return ApplicationResult.Failure("El tamaño de página no puede ser mayor a 100 registros.");

        return ApplicationResult.Success();
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