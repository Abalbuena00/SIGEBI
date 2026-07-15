using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ConsultarSolicitudesPendientesHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ConsultarSolicitudesPendientesHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>> HandleAsync(
        ConsultarSolicitudesPendientesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.UsuarioConsultorId <= 0)
            return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Failure(
                "El usuario consultor es obligatorio.");

        var usuarioConsultor = await _usuarioRepository.ObtenerPorIdAsync(
            query.UsuarioConsultorId,
            cancellationToken);

        if (usuarioConsultor is null)
            return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Failure(
                "El usuario consultor no fue encontrado.");

        if (usuarioConsultor.Estado != EstadoUsuario.Activo)
            return ApplicationResult<IReadOnlyList<SolicitudPrestamoDto>>.Failure(
                "El usuario consultor no se encuentra activo.");

        var solicitudes = await _solicitudPrestamoRepository.ObtenerPendientesAsync(
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