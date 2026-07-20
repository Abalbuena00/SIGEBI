using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class RechazarSolicitudPrestamoHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RechazarSolicitudPrestamoHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IEjemplarRepository ejemplarRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _ejemplarRepository = ejemplarRepository;
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

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            solicitud.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult.Failure("El ejemplar asociado a la solicitud no fue encontrado.");

        if (ejemplar.Estado != EstadoEjemplar.Reservado)
            return ApplicationResult.Failure("El ejemplar asociado a la solicitud no se encuentra reservado.");

        var resultadoRechazo = solicitud.Rechazar(
            command.UsuarioRechazaId,
            command.Motivo);

        if (!resultadoRechazo.IsSuccess)
            return ApplicationResult.Failure(resultadoRechazo.Error!);

        var resultadoLiberarEjemplar = ejemplar.LiberarReserva();

        if (!resultadoLiberarEjemplar.IsSuccess)
            return ApplicationResult.Failure(resultadoLiberarEjemplar.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}