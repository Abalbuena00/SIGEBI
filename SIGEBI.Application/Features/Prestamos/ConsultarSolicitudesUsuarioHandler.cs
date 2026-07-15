using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ConsultarSolicitudesUsuarioHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ConsultarSolicitudesUsuarioHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>> HandleAsync(
        ConsultarSolicitudesUsuarioQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.UsuarioId <= 0)
            return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Failure(
                "El usuario es obligatorio.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            query.UsuarioId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Failure(
                "El usuario no fue encontrado.");

        if (usuario.Estado != EstadoUsuario.Activo)
            return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Failure(
                "El usuario no se encuentra activo.");

        var solicitudes = await _solicitudPrestamoRepository.ObtenerPorUsuarioAsync(
            query.UsuarioId,
            cancellationToken);

        var resultado = solicitudes
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Success(resultado);
    }

    private static SolicitudPrestamoDto MapearADto(SolicitudPrestamo solicitud)
    {
        return new SolicitudPrestamoDto
        {
            Id = solicitud.Id,
            UsuarioId = solicitud.UsuarioId,
            EjemplarId = solicitud.EjemplarId,
            FechaSolicitud = solicitud.FechaSolicitud,
            FechaExpiracionSolicitud = solicitud.FechaExpiracionSolicitud,
            Estado = (int)solicitud.Estado,
            EstadoDescripcion = solicitud.Estado.ToString(),
            FechaAprobacion = solicitud.FechaAprobacion,
            FechaRechazo = solicitud.FechaRechazo,
            FechaCompletada = solicitud.FechaCompletada,
            UsuarioAprobadorId = solicitud.UsuarioAprobadorId,
            MotivoRechazo = solicitud.MotivoRechazo
        };
    }
}