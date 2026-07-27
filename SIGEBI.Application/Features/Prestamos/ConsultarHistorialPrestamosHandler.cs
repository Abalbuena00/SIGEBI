using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ConsultarHistorialPrestamosHandler
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRecursoBibliograficoRepository _recursoRepository;

    public ConsultarHistorialPrestamosHandler(
        IPrestamoRepository prestamoRepository,
        IUsuarioRepository usuarioRepository,
        IRecursoBibliograficoRepository recursoRepository)
    {
        _prestamoRepository = prestamoRepository;
        _usuarioRepository = usuarioRepository;
        _recursoRepository = recursoRepository;
    }

    public async Task<ApplicationResult<PagedResult<PrestamoDto>>> HandleAsync(
        ConsultarHistorialPrestamosQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var validacion = ValidarQuery(query);
        if (!validacion.IsSuccess)
            return ApplicationResult<PagedResult<PrestamoDto>>.Failure(validacion.Error!);

        if (query.UsuarioId.HasValue)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                query.UsuarioId.Value, cancellationToken);
            if (usuario is null)
                return ApplicationResult<PagedResult<PrestamoDto>>.Failure(
                    "El usuario no fue encontrado.");
        }

        if (query.RecursoBibliograficoId.HasValue)
        {
            var recurso = await _recursoRepository.ObtenerPorIdAsync(
                query.RecursoBibliograficoId.Value, cancellationToken);
            if (recurso is null)
                return ApplicationResult<PagedResult<PrestamoDto>>.Failure(
                    "El recurso bibliográfico no fue encontrado.");
        }

        var consulta = await _prestamoRepository.ConsultarHistorialAsync(
            query.UsuarioId, query.RecursoBibliograficoId, query.EjemplarId,
            query.Estado, query.FechaDesde, query.FechaHasta,
            query.PageNumber, query.PageSize, cancellationToken);
        var items = consulta.Items.Select(MapearADto).ToList();
        var resultado = new PagedResult<PrestamoDto>(
            items, consulta.TotalItems, query.PageNumber, query.PageSize);

        return ApplicationResult<PagedResult<PrestamoDto>>.Success(resultado);
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

    private static ApplicationResult ValidarQuery(ConsultarHistorialPrestamosQuery query)
    {
        if (query.PageNumber <= 0)
            return ApplicationResult.Failure("El número de página debe ser mayor que cero.");
        if (query.PageSize <= 0)
            return ApplicationResult.Failure("El tamaño de página debe ser mayor que cero.");
        if (query.PageSize > 100)
            return ApplicationResult.Failure("El tamaño de página no puede superar 100 registros.");
        if (query.UsuarioId.HasValue && query.UsuarioId.Value <= 0)
            return ApplicationResult.Failure("El usuario indicado no es válido.");
        if (query.RecursoBibliograficoId.HasValue && query.RecursoBibliograficoId.Value <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico indicado no es válido.");
        if (query.EjemplarId.HasValue && query.EjemplarId.Value <= 0)
            return ApplicationResult.Failure("El ejemplar indicado no es válido.");
        if (query.Estado.HasValue && !Enum.IsDefined(typeof(EstadoPrestamo), query.Estado.Value))
            return ApplicationResult.Failure("El estado de préstamo indicado no es válido.");
        if (query.FechaDesde.HasValue && query.FechaHasta.HasValue &&
            query.FechaDesde.Value > query.FechaHasta.Value)
            return ApplicationResult.Failure("La fecha desde no puede ser mayor que la fecha hasta.");

        return ApplicationResult.Success();
    }
}
