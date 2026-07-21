using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ExpirarSolicitudesPrestamoHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IReservaTemporalRepository _reservaTemporalRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExpirarSolicitudesPrestamoHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IReservaTemporalRepository reservaTemporalRepository,
        IEjemplarRepository ejemplarRepository,
        IUnitOfWork unitOfWork)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _reservaTemporalRepository = reservaTemporalRepository;
        _ejemplarRepository = ejemplarRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ResultadoExpiracionSolicitudesDto>> HandleAsync(
        ExpirarSolicitudesPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        DateTime fechaReferencia = command.FechaReferencia ?? DateTime.UtcNow;

        var resultado = new ResultadoExpiracionSolicitudesDto();

        await ExpirarSolicitudesPendientesAsync(
            fechaReferencia,
            resultado,
            cancellationToken);

        await ExpirarReservasTemporalesAsync(
            fechaReferencia,
            resultado,
            cancellationToken);

        if (resultado.SolicitudesPendientesVencidas > 0 ||
            resultado.SolicitudesAprobadasVencidas > 0 ||
            resultado.ReservasTemporalesVencidas > 0 ||
            resultado.EjemplaresLiberados > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ApplicationResult<ResultadoExpiracionSolicitudesDto>.Success(resultado);
    }

    private async Task ExpirarSolicitudesPendientesAsync(
        DateTime fechaReferencia,
        ResultadoExpiracionSolicitudesDto resultado,
        CancellationToken cancellationToken)
    {
        var solicitudesVencidas = await _solicitudPrestamoRepository.ObtenerVenciblesAsync(
            fechaReferencia,
            cancellationToken);

        foreach (var solicitud in solicitudesVencidas)
        {
            var resultadoVencimiento = solicitud.Vencer();

            if (!resultadoVencimiento.IsSuccess)
                continue;

            resultado.SolicitudesPendientesVencidas++;

            var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
                solicitud.EjemplarId,
                cancellationToken);

            if (ejemplar is null || ejemplar.Estado != EstadoEjemplar.Reservado)
                continue;

            var resultadoLiberacion = ejemplar.LiberarReserva();

            if (!resultadoLiberacion.IsSuccess)
                continue;

            resultado.EjemplaresLiberados++;
        }
    }

    private async Task ExpirarReservasTemporalesAsync(
        DateTime fechaReferencia,
        ResultadoExpiracionSolicitudesDto resultado,
        CancellationToken cancellationToken)
    {
        var reservasVencidas = await _reservaTemporalRepository.ObtenerVencidasAsync(
            fechaReferencia,
            cancellationToken);

        foreach (var reservaTemporal in reservasVencidas)
        {
            var resultadoVencimientoReserva = reservaTemporal.Vencer();

            if (!resultadoVencimientoReserva.IsSuccess)
                continue;

            resultado.ReservasTemporalesVencidas++;

            var solicitud = await _solicitudPrestamoRepository.ObtenerPorIdAsync(
                reservaTemporal.SolicitudPrestamoId,
                cancellationToken);

            if (solicitud is not null)
            {
                var resultadoVencimientoSolicitud = solicitud.Vencer();

                if (resultadoVencimientoSolicitud.IsSuccess)
                    resultado.SolicitudesAprobadasVencidas++;
            }

            var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
                reservaTemporal.EjemplarId,
                cancellationToken);

            if (ejemplar is null || ejemplar.Estado != EstadoEjemplar.Reservado)
                continue;

            var resultadoLiberacion = ejemplar.LiberarReserva();

            if (!resultadoLiberacion.IsSuccess)
                continue;

            resultado.EjemplaresLiberados++;
        }
    }
}