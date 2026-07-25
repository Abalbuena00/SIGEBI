using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Penalizaciones;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Penalizaciones;

public sealed class ConsultarPenalizacionesUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;

    public ConsultarPenalizacionesUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        IPenalizacionRepository penalizacionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _penalizacionRepository = penalizacionRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<PenalizacionDto>>> HandleAsync(
        ConsultarPenalizacionesUsuarioQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.UsuarioId <= 0)
            return ApplicationResult<IReadOnlyList<PenalizacionDto>>.Failure("El usuario es obligatorio.");

        if (query.Estado.HasValue &&
            !Enum.IsDefined(typeof(EstadoPenalizacion), query.Estado.Value))
        {
            return ApplicationResult<IReadOnlyList<PenalizacionDto>>.Failure(
                "El estado de penalización indicado no es válido.");
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            query.UsuarioId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult<IReadOnlyList<PenalizacionDto>>.Failure("El usuario no fue encontrado.");

        var penalizaciones = await _penalizacionRepository.ObtenerPorUsuarioAsync(
            query.UsuarioId,
            query.Estado,
            cancellationToken);

        var resultado = penalizaciones
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<PenalizacionDto>>.Success(resultado);
    }

    private static PenalizacionDto MapearADto(Penalizacion penalizacion)
    {
        return new PenalizacionDto
        {
            Id = penalizacion.Id,
            UsuarioId = penalizacion.UsuarioId,
            PrestamoId = penalizacion.PrestamoId,
            DiasSuspension = penalizacion.DiasSuspension,
            FechaInicio = penalizacion.FechaInicio,
            FechaFin = penalizacion.FechaFin,
            Estado = (int)penalizacion.Estado,
            EstadoDescripcion = penalizacion.Estado.ToString(),
            FechaResolucion = penalizacion.FechaResolucion,
            UsuarioResolutorId = penalizacion.UsuarioResolutorId,
            MotivoResolucion = penalizacion.MotivoResolucion
        };
    }
}
