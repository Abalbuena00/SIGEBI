using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Incidencias;
using SIGEBI.Application.DTOs.Penalizaciones;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Application.DTOs.Seguridad;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class ConsultarHistorialUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISolicitudPrestamoRepository _solicitudRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IIncidenciaEjemplarRepository _incidenciaRepository;

    public ConsultarHistorialUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        ISolicitudPrestamoRepository solicitudRepository,
        IPrestamoRepository prestamoRepository,
        IPenalizacionRepository penalizacionRepository,
        IIncidenciaEjemplarRepository incidenciaRepository)
    {
        _usuarioRepository = usuarioRepository;
        _solicitudRepository = solicitudRepository;
        _prestamoRepository = prestamoRepository;
        _penalizacionRepository = penalizacionRepository;
        _incidenciaRepository = incidenciaRepository;
    }

    public async Task<ApplicationResult<HistorialUsuarioDto>> HandleAsync(
        ConsultarHistorialUsuarioQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.UsuarioId <= 0)
            return ApplicationResult<HistorialUsuarioDto>.Failure("El usuario es obligatorio.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(query.UsuarioId, cancellationToken);

        if (usuario is null)
            return ApplicationResult<HistorialUsuarioDto>.Failure("El usuario no fue encontrado.");

        var solicitudes = await _solicitudRepository.ObtenerPorUsuarioAsync(usuario.Id, cancellationToken);
        var prestamos = await _prestamoRepository.ConsultarHistorialAsync(
            usuario.Id, null, null, null, null, null, 1, 100, cancellationToken);
        var penalizaciones = await _penalizacionRepository.ObtenerPorUsuarioAsync(
            usuario.Id, null, cancellationToken);
        var incidencias = await _incidenciaRepository.ObtenerPorUsuarioAsync(usuario.Id, cancellationToken);

        var resultado = new HistorialUsuarioDto
        {
            UsuarioId = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Correo = usuario.Correo,
            Matricula = usuario.Matricula,
            NumeroEmpleado = usuario.NumeroEmpleado,
            Estado = (int)usuario.Estado,
            EstadoDescripcion = usuario.Estado.ToString(),
            Solicitudes = solicitudes.Select(MapearSolicitud).ToList(),
            Prestamos = prestamos.Items.Select(MapearPrestamo).ToList(),
            Penalizaciones = penalizaciones.Select(MapearPenalizacion).ToList(),
            Incidencias = incidencias.Select(MapearIncidencia).ToList()
        };

        return ApplicationResult<HistorialUsuarioDto>.Success(resultado);
    }

    private static SolicitudPrestamoDto MapearSolicitud(SolicitudPrestamo solicitud) => new()
    {
        Id = solicitud.Id, UsuarioId = solicitud.UsuarioId, EjemplarId = solicitud.EjemplarId,
        FechaSolicitud = solicitud.FechaSolicitud,
        FechaExpiracionSolicitud = solicitud.FechaExpiracionSolicitud,
        Estado = (int)solicitud.Estado, EstadoDescripcion = solicitud.Estado.ToString(),
        FechaAprobacion = solicitud.FechaAprobacion, FechaRechazo = solicitud.FechaRechazo,
        FechaCompletada = solicitud.FechaCompletada,
        UsuarioAprobadorId = solicitud.UsuarioAprobadorId, MotivoRechazo = solicitud.MotivoRechazo
    };

    private static PrestamoDto MapearPrestamo(Prestamo prestamo) => new()
    {
        Id = prestamo.Id, UsuarioId = prestamo.UsuarioId, EjemplarId = prestamo.EjemplarId,
        SolicitudPrestamoId = prestamo.SolicitudPrestamoId,
        UsuarioBibliotecarioId = prestamo.UsuarioBibliotecarioId,
        FechaInicio = prestamo.FechaInicio,
        FechaLimiteDevolucion = prestamo.FechaLimiteDevolucion,
        FechaDevolucion = prestamo.FechaDevolucion,
        Estado = (int)prestamo.Estado, EstadoDescripcion = prestamo.Estado.ToString(),
        EstaVencido = prestamo.EstaVencido()
    };

    private static PenalizacionDto MapearPenalizacion(Penalizacion penalizacion) => new()
    {
        Id = penalizacion.Id, UsuarioId = penalizacion.UsuarioId,
        PrestamoId = penalizacion.PrestamoId, DiasSuspension = penalizacion.DiasSuspension,
        FechaInicio = penalizacion.FechaInicio, FechaFin = penalizacion.FechaFin,
        Estado = (int)penalizacion.Estado, EstadoDescripcion = penalizacion.Estado.ToString(),
        FechaResolucion = penalizacion.FechaResolucion,
        UsuarioResolutorId = penalizacion.UsuarioResolutorId,
        MotivoResolucion = penalizacion.MotivoResolucion
    };

    private static IncidenciaEjemplarDto MapearIncidencia(IncidenciaEjemplar incidencia) => new()
    {
        Id = incidencia.Id, EjemplarId = incidencia.EjemplarId,
        PrestamoId = incidencia.PrestamoId, UsuarioReportaId = incidencia.UsuarioReportaId,
        Tipo = (int)incidencia.Tipo, TipoDescripcion = incidencia.Tipo.ToString(),
        Descripcion = incidencia.Descripcion, FechaRegistro = incidencia.FechaRegistro,
        Cerrada = incidencia.Cerrada, FechaCierre = incidencia.FechaCierre,
        UsuarioCierreId = incidencia.UsuarioCierreId
    };
}
