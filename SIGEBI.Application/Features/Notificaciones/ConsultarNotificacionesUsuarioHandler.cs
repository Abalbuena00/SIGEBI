using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Notificaciones;
using SIGEBI.Domain.Entities.Notificaciones;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Notificaciones;

public sealed class ConsultarNotificacionesUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly INotificacionRepository _notificacionRepository;

    public ConsultarNotificacionesUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        INotificacionRepository notificacionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _notificacionRepository = notificacionRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<NotificacionDto>>> HandleAsync(
        ConsultarNotificacionesUsuarioQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.UsuarioId <= 0)
            return ApplicationResult<IReadOnlyList<NotificacionDto>>.Failure("El usuario es obligatorio.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            query.UsuarioId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult<IReadOnlyList<NotificacionDto>>.Failure("El usuario no fue encontrado.");

        var notificaciones = query.SoloNoLeidas
            ? await _notificacionRepository.ObtenerNoLeidasPorUsuarioAsync(
                query.UsuarioId,
                cancellationToken)
            : await _notificacionRepository.ObtenerPorUsuarioAsync(
                query.UsuarioId,
                cancellationToken);

        var resultado = notificaciones
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<NotificacionDto>>.Success(resultado);
    }

    private static NotificacionDto MapearADto(Notificacion notificacion)
    {
        return new NotificacionDto
        {
            Id = notificacion.Id,
            UsuarioDestinatarioId = notificacion.UsuarioDestinatarioId,
            Tipo = (int)notificacion.Tipo,
            TipoDescripcion = notificacion.Tipo.ToString(),
            Estado = (int)notificacion.Estado,
            EstadoDescripcion = notificacion.Estado.ToString(),
            Titulo = notificacion.Titulo,
            Mensaje = notificacion.Mensaje,
            FechaEnvio = notificacion.FechaEnvio,
            FechaLectura = notificacion.FechaLectura,
            EntidadReferencia = notificacion.EntidadReferencia,
            EntidadReferenciaId = notificacion.EntidadReferenciaId
        };
    }
}