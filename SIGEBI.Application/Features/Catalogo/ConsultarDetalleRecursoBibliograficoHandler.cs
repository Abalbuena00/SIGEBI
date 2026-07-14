using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Catalogo;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarDetalleRecursoBibliograficoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;

    public ConsultarDetalleRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
    }

  

    public async Task<ApplicationResult<RecursoBibliograficoDto>> HandleAsync(
        ConsultarDetalleRecursoBibliograficoQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Id <= 0)
            return ApplicationResult<RecursoBibliograficoDto>.Failure(
                "El identificador del recurso bibliográfico es obligatorio.");

        var recurso = await _recursoBibliograficoRepository.ObtenerDetallePorIdAsync(
            query.Id,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult<RecursoBibliograficoDto>.Failure(
                "El recurso bibliográfico no fue encontrado.");

        return ApplicationResult<RecursoBibliograficoDto>.Success(
            MapearADto(recurso));
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