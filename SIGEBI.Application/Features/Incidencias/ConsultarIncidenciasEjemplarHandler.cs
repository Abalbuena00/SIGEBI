using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Penalizaciones;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Incidencias;

public sealed class ConsultarIncidenciasEjemplarHandler
{
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IIncidenciaEjemplarRepository _incidenciaRepository;

    public ConsultarIncidenciasEjemplarHandler(
        IEjemplarRepository ejemplarRepository,
        IIncidenciaEjemplarRepository incidenciaRepository)
    {
        _ejemplarRepository = ejemplarRepository;
        _incidenciaRepository = incidenciaRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<IncidenciaEjemplarDto>>> HandleAsync(
        ConsultarIncidenciasEjemplarQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.EjemplarId <= 0)
        {
            return ApplicationResult<IReadOnlyList<IncidenciaEjemplarDto>>.Failure(
                "El ejemplar es obligatorio.");
        }

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            query.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
        {
            return ApplicationResult<IReadOnlyList<IncidenciaEjemplarDto>>.Failure(
                "El ejemplar no fue encontrado.");
        }

        var incidencias = await _incidenciaRepository.ObtenerPorEjemplarAsync(
            query.EjemplarId,
            query.Cerrada,
            cancellationToken);

        var resultado = incidencias
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<IncidenciaEjemplarDto>>.Success(resultado);
    }

    private static IncidenciaEjemplarDto MapearADto(
        IncidenciaEjemplar incidencia)
    {
        return new IncidenciaEjemplarDto
        {
            Id = incidencia.Id,
            EjemplarId = incidencia.EjemplarId,
            PrestamoId = incidencia.PrestamoId,
            UsuarioReportaId = incidencia.UsuarioReportaId,
            Tipo = (int)incidencia.Tipo,
            TipoDescripcion = incidencia.Tipo.ToString(),
            Descripcion = incidencia.Descripcion,
            FechaRegistro = incidencia.FechaRegistro,
            Cerrada = incidencia.Cerrada,
            FechaCierre = incidencia.FechaCierre,
            UsuarioCierreId = incidencia.UsuarioCierreId
        };
    }
}
