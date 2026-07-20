using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class AprobarSolicitudPrestamoHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPoliticaPrestamoRepository _politicaPrestamoRepository;
    private readonly IReservaTemporalRepository _reservaTemporalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AprobarSolicitudPrestamoHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IEjemplarRepository ejemplarRepository,
        IUsuarioRepository usuarioRepository,
        IPoliticaPrestamoRepository politicaPrestamoRepository,
        IReservaTemporalRepository reservaTemporalRepository,
        IUnitOfWork unitOfWork)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _ejemplarRepository = ejemplarRepository;
        _usuarioRepository = usuarioRepository;
        _politicaPrestamoRepository = politicaPrestamoRepository;
        _reservaTemporalRepository = reservaTemporalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        AprobarSolicitudPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.SolicitudPrestamoId <= 0)
            return ApplicationResult.Failure("La solicitud de préstamo es obligatoria.");

        if (command.UsuarioAprobadorId <= 0)
            return ApplicationResult.Failure("El usuario aprobador es obligatorio.");

        var solicitud = await _solicitudPrestamoRepository.ObtenerPorIdAsync(
            command.SolicitudPrestamoId,
            cancellationToken);

        if (solicitud is null)
            return ApplicationResult.Failure("La solicitud de préstamo no fue encontrada.");

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            solicitud.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult.Failure("El ejemplar asociado a la solicitud no fue encontrado.");

        if (solicitud.FechaExpiracionSolicitud < DateTime.UtcNow)
        {
            var resultadoVencimiento = solicitud.Vencer();

            if (!resultadoVencimiento.IsSuccess)
                return ApplicationResult.Failure(resultadoVencimiento.Error!);

            if (ejemplar.Estado == EstadoEjemplar.Reservado)
            {
                var resultadoLiberarEjemplar = ejemplar.LiberarReserva();

                if (!resultadoLiberarEjemplar.IsSuccess)
                    return ApplicationResult.Failure(resultadoLiberarEjemplar.Error!);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApplicationResult.Failure("La solicitud de préstamo ya se encuentra vencida.");
        }

        if (ejemplar.Estado != EstadoEjemplar.Reservado)
            return ApplicationResult.Failure("El ejemplar asociado a la solicitud no se encuentra reservado.");

        var usuarioAprobador = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioAprobadorId,
            cancellationToken);

        if (usuarioAprobador is null)
            return ApplicationResult.Failure("El usuario aprobador no fue encontrado.");

        if (usuarioAprobador.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario aprobador no se encuentra activo.");

        TipoMiembro tipoMiembroSolicitante = await DeterminarTipoMiembroSolicitanteAsync(
            solicitud.UsuarioId,
            cancellationToken);

        var politica = await _politicaPrestamoRepository.ObtenerPorTipoMiembroAsync(
            tipoMiembroSolicitante,
            cancellationToken);

        if (politica is null)
            return ApplicationResult.Failure("No existe una política de préstamo configurada para el usuario solicitante.");

        var resultadoAprobacion = solicitud.Aprobar(command.UsuarioAprobadorId);

        if (!resultadoAprobacion.IsSuccess)
            return ApplicationResult.Failure(resultadoAprobacion.Error!);

        var reservaTemporal = new ReservaTemporal(
            solicitud.Id,
            solicitud.EjemplarId,
            politica.HorasReservaTemporal);

        await _reservaTemporalRepository.AgregarAsync(
            reservaTemporal,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private async Task<TipoMiembro> DeterminarTipoMiembroSolicitanteAsync(
        int usuarioId,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
            return TipoMiembro.Estudiante;

        if (!string.IsNullOrWhiteSpace(usuario.NumeroEmpleado))
            return TipoMiembro.Docente;

        return TipoMiembro.Estudiante;
    }
}