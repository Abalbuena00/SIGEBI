using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class CancelarSolicitudPrestamoHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IReservaTemporalRepository _reservaTemporalRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelarSolicitudPrestamoHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IUsuarioRepository usuarioRepository,
        IReservaTemporalRepository reservaTemporalRepository,
        IEjemplarRepository ejemplarRepository,
        IUnitOfWork unitOfWork)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _usuarioRepository = usuarioRepository;
        _reservaTemporalRepository = reservaTemporalRepository;
        _ejemplarRepository = ejemplarRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        CancelarSolicitudPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.SolicitudPrestamoId <= 0)
            return ApplicationResult.Failure("La solicitud de préstamo es obligatoria.");

        if (command.UsuarioCancelaId <= 0)
            return ApplicationResult.Failure("El usuario que cancela la solicitud es obligatorio.");

        var solicitud = await _solicitudPrestamoRepository.ObtenerPorIdAsync(
            command.SolicitudPrestamoId,
            cancellationToken);

        if (solicitud is null)
            return ApplicationResult.Failure("La solicitud de préstamo no fue encontrada.");

        var usuarioCancela = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioCancelaId,
            cancellationToken);

        if (usuarioCancela is null)
            return ApplicationResult.Failure("El usuario que cancela la solicitud no fue encontrado.");

        if (usuarioCancela.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario que cancela la solicitud no se encuentra activo.");

        if (solicitud.UsuarioId != command.UsuarioCancelaId)
            return ApplicationResult.Failure("El usuario no puede cancelar una solicitud que no le pertenece.");

        if (solicitud.Estado != EstadoSolicitudPrestamo.Pendiente &&
            solicitud.Estado != EstadoSolicitudPrestamo.AprobadaPendienteRetiro)
        {
            return ApplicationResult.Failure("La solicitud no se encuentra en un estado cancelable.");
        }

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            solicitud.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult.Failure("El ejemplar asociado a la solicitud no fue encontrado.");

        if (ejemplar.Estado != EstadoEjemplar.Reservado)
            return ApplicationResult.Failure("El ejemplar asociado a la solicitud no se encuentra reservado.");

        if (solicitud.Estado == EstadoSolicitudPrestamo.AprobadaPendienteRetiro)
        {
            var reservaTemporal = await _reservaTemporalRepository.ObtenerActivaPorEjemplarAsync(
                solicitud.EjemplarId,
                cancellationToken);

            if (reservaTemporal is null || reservaTemporal.SolicitudPrestamoId != solicitud.Id)
                return ApplicationResult.Failure("No existe una reserva temporal activa para esta solicitud.");

            var resultadoCancelarReserva = reservaTemporal.Cancelar();

            if (!resultadoCancelarReserva.IsSuccess)
                return ApplicationResult.Failure(resultadoCancelarReserva.Error!);
        }

        var resultadoLiberarEjemplar = ejemplar.LiberarReserva();

        if (!resultadoLiberarEjemplar.IsSuccess)
            return ApplicationResult.Failure(resultadoLiberarEjemplar.Error!);

        var resultadoCancelarSolicitud = solicitud.Cancelar();

        if (!resultadoCancelarSolicitud.IsSuccess)
            return ApplicationResult.Failure(resultadoCancelarSolicitud.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}