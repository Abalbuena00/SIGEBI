using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ConsultarPrestamosActivosUsuarioHandler
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ConsultarPrestamosActivosUsuarioHandler(
        IPrestamoRepository prestamoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _prestamoRepository = prestamoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<PrestamoDto>>> HandleAsync(
        ConsultarPrestamosActivosUsuarioQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.UsuarioId <= 0)
            return ApplicationResult<IReadOnlyList<PrestamoDto>>.Failure(
                "El usuario es obligatorio.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            query.UsuarioId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult<IReadOnlyList<PrestamoDto>>.Failure(
                "El usuario no fue encontrado.");

        if (usuario.Estado != EstadoUsuario.Activo)
            return ApplicationResult<IReadOnlyList<PrestamoDto>>.Failure(
                "El usuario no se encuentra activo.");

        var prestamos = await _prestamoRepository.ObtenerActivosPorUsuarioAsync(
            query.UsuarioId,
            cancellationToken);

        var resultado = prestamos
            .Select(MapearADto)
            .ToList();

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