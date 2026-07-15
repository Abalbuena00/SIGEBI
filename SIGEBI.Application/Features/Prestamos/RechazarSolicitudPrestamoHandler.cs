using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class RechazarSolicitudPrestamoHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RechazarSolicitudPrestamoHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        RechazarSolicitudPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.SolicitudPrestamoId <= 0)
            return ApplicationResult.Failure("La solicitud de préstamo es obligatoria.");

        if (command.UsuarioRechazaId <= 0)
            return ApplicationResult.Failure("El usuario que rechaza la solicitud es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Motivo))
            return ApplicationResult.Failure("Debe indicar el motivo del rechazo.");

        var solicitud = await _solicitudPrestamoRepository.ObtenerPorIdAsync(
            command.SolicitudPrestamoId,
            cancellationToken);

        if (solicitud is null)
            return ApplicationResult.Failure("La solicitud de préstamo no fue encontrada.");

        var usuarioRechaza = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioRechazaId,
            cancellationToken);

        if (usuarioRechaza is null)
            return ApplicationResult.Failure("El usuario que rechaza la solicitud no fue encontrado.");

        if (usuarioRechaza.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario que rechaza la solicitud no se encuentra activo.");

        var resultado = solicitud.Rechazar(
            command.UsuarioRechazaId,
            command.Motivo);

        if (!resultado.IsSuccess)
            return ApplicationResult.Failure(resultado.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}