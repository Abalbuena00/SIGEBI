using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Catalogo;
using SIGEBI.Application.DTOs.Incidencias;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarHistorialRecursoBibliograficoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IIncidenciaEjemplarRepository _incidenciaRepository;

    public ConsultarHistorialRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoRepository,
        IEjemplarRepository ejemplarRepository,
        IPrestamoRepository prestamoRepository,
        IIncidenciaEjemplarRepository incidenciaRepository)
    {
        _recursoRepository = recursoRepository;
        _ejemplarRepository = ejemplarRepository;
        _prestamoRepository = prestamoRepository;
        _incidenciaRepository = incidenciaRepository;
    }

    public async Task<ApplicationResult<HistorialRecursoBibliograficoDto>> HandleAsync(
        ConsultarHistorialRecursoBibliograficoQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.RecursoBibliograficoId <= 0)
        {
            return ApplicationResult<HistorialRecursoBibliograficoDto>.Failure(
                "El recurso bibliográfico es obligatorio.");
        }

        var recurso = await _recursoRepository.ObtenerPorIdAsync(
            query.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
        {
            return ApplicationResult<HistorialRecursoBibliograficoDto>.Failure(
                "El recurso bibliográfico no fue encontrado.");
        }

        var historiales = await _ejemplarRepository.ObtenerHistorialEstadosPorRecursoAsync(
            recurso.Id,
            cancellationToken);

        var prestamos = await _prestamoRepository.ConsultarHistorialAsync(
            usuarioId: null,
            recursoBibliograficoId: recurso.Id,
            ejemplarId: null,
            estado: null,
            fechaDesde: null,
            fechaHasta: null,
            pageNumber: 1,
            pageSize: 100,
            cancellationToken: cancellationToken);

        var incidencias = await _incidenciaRepository.ObtenerPorRecursoAsync(
            recurso.Id,
            cancellationToken);

        var resultado = new HistorialRecursoBibliograficoDto
        {
            RecursoBibliograficoId = recurso.Id,
            CodigoInterno = recurso.CodigoInterno,
            Titulo = recurso.Titulo,
            HistorialEstados = historiales.Select(MapearHistorial).ToList(),
            Prestamos = prestamos.Items.Select(MapearPrestamo).ToList(),
            Incidencias = incidencias.Select(MapearIncidencia).ToList()
        };

        return ApplicationResult<HistorialRecursoBibliograficoDto>.Success(resultado);
    }

    private static HistorialEstadoEjemplarDto MapearHistorial(
        HistorialEstadoEjemplar historial)
    {
        return new HistorialEstadoEjemplarDto
        {
            Id = historial.Id,
            EjemplarId = historial.EjemplarId,
            EstadoAnterior = historial.EstadoAnterior.HasValue
                ? (int)historial.EstadoAnterior.Value
                : null,
            EstadoAnteriorDescripcion = historial.EstadoAnterior?.ToString(),
            EstadoNuevo = (int)historial.EstadoNuevo,
            EstadoNuevoDescripcion = historial.EstadoNuevo.ToString(),
            FechaCambio = historial.FechaCambio,
            UsuarioResponsableId = historial.UsuarioResponsableId,
            Motivo = historial.Motivo
        };
    }

    private static PrestamoDto MapearPrestamo(Prestamo prestamo)
    {
        return new PrestamoDto
        {
            Id = prestamo.Id,
            UsuarioId = prestamo.UsuarioId,
            EjemplarId = prestamo.EjemplarId,
            SolicitudPrestamoId = prestamo.SolicitudPrestamoId,
            UsuarioBibliotecarioId = prestamo.UsuarioBibliotecarioId,
            FechaInicio = prestamo.FechaInicio,
            FechaLimiteDevolucion = prestamo.FechaLimiteDevolucion,
            FechaDevolucion = prestamo.FechaDevolucion,
            Estado = (int)prestamo.Estado,
            EstadoDescripcion = prestamo.Estado.ToString(),
            EstaVencido = prestamo.EstaVencido()
        };
    }

    private static IncidenciaEjemplarDto MapearIncidencia(
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