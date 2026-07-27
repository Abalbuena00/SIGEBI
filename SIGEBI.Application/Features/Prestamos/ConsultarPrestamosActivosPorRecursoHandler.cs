using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ConsultarPrestamosActivosPorRecursoHandler
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IRecursoBibliograficoRepository _recursoRepository;

    public ConsultarPrestamosActivosPorRecursoHandler(
        IPrestamoRepository prestamoRepository,
        IRecursoBibliograficoRepository recursoRepository)
    {
        _prestamoRepository = prestamoRepository;
        _recursoRepository = recursoRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<PrestamoDto>>> HandleAsync(
        ConsultarPrestamosActivosPorRecursoQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.RecursoBibliograficoId <= 0)
            return ApplicationResult<IReadOnlyList<PrestamoDto>>.Failure(
                "El recurso bibliográfico es obligatorio.");

        var recurso = await _recursoRepository.ObtenerPorIdAsync(
            query.RecursoBibliograficoId, cancellationToken);
        if (recurso is null)
            return ApplicationResult<IReadOnlyList<PrestamoDto>>.Failure(
                "El recurso bibliográfico no fue encontrado.");

        var prestamos = await _prestamoRepository.ObtenerAbiertosPorRecursoAsync(
            query.RecursoBibliograficoId, cancellationToken);
        var resultado = prestamos.Select(MapearADto).ToList();

        return ApplicationResult<IReadOnlyList<PrestamoDto>>.Success(resultado);
    }

    private static PrestamoDto MapearADto(Prestamo prestamo)
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
}
