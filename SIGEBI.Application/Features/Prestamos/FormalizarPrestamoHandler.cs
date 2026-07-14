using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class FormalizarPrestamoHandler
{
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IReservaTemporalRepository _reservaTemporalRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPoliticaPrestamoRepository _politicaPrestamoRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FormalizarPrestamoHandler(
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IReservaTemporalRepository reservaTemporalRepository,
        IEjemplarRepository ejemplarRepository,
        IUsuarioRepository usuarioRepository,
        IPoliticaPrestamoRepository politicaPrestamoRepository,
        IPrestamoRepository prestamoRepository,
        IUnitOfWork unitOfWork)
    {
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _reservaTemporalRepository = reservaTemporalRepository;
        _ejemplarRepository = ejemplarRepository;
        _usuarioRepository = usuarioRepository;
        _politicaPrestamoRepository = politicaPrestamoRepository;
        _prestamoRepository = prestamoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        FormalizarPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.SolicitudPrestamoId <= 0)
            return ApplicationResult<int>.Failure("La solicitud de préstamo es obligatoria.");

        if (command.UsuarioBibliotecarioId <= 0)
            return ApplicationResult<int>.Failure("El bibliotecario responsable es obligatorio.");

        var solicitud = await _solicitudPrestamoRepository.ObtenerPorIdAsync(
            command.SolicitudPrestamoId,
            cancellationToken);

        if (solicitud is null)
            return ApplicationResult<int>.Failure("La solicitud de préstamo no fue encontrada.");

        if (solicitud.Estado != EstadoSolicitudPrestamo.AprobadaPendienteRetiro)
            return ApplicationResult<int>.Failure("La solicitud no está aprobada pendiente de retiro.");

        var bibliotecario = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioBibliotecarioId,
            cancellationToken);

        if (bibliotecario is null)
            return ApplicationResult<int>.Failure("El bibliotecario responsable no fue encontrado.");

        if (bibliotecario.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El bibliotecario responsable no se encuentra activo.");

        var usuarioSolicitante = await _usuarioRepository.ObtenerPorIdAsync(
            solicitud.UsuarioId,
            cancellationToken);

        if (usuarioSolicitante is null)
            return ApplicationResult<int>.Failure("El usuario solicitante no fue encontrado.");

        if (usuarioSolicitante.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El usuario solicitante no se encuentra activo.");

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            solicitud.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult<int>.Failure("El ejemplar asociado a la solicitud no fue encontrado.");

        var reservaTemporal = await _reservaTemporalRepository.ObtenerActivaPorEjemplarAsync(
            solicitud.EjemplarId,
            cancellationToken);

        if (reservaTemporal is null || reservaTemporal.SolicitudPrestamoId != solicitud.Id)
            return ApplicationResult<int>.Failure("No existe una reserva temporal activa para esta solicitud.");

        if (!reservaTemporal.EstaVigente())
        {
            var resultadoVencerReserva = reservaTemporal.Vencer();

            if (!resultadoVencerReserva.IsSuccess)
                return ApplicationResult<int>.Failure(resultadoVencerReserva.Error!);

            var resultadoVencerSolicitud = solicitud.Vencer();

            if (!resultadoVencerSolicitud.IsSuccess)
                return ApplicationResult<int>.Failure(resultadoVencerSolicitud.Error!);

            var resultadoLiberarEjemplar = ejemplar.LiberarReserva();

            if (!resultadoLiberarEjemplar.IsSuccess)
                return ApplicationResult<int>.Failure(resultadoLiberarEjemplar.Error!);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApplicationResult<int>.Failure("La reserva temporal ya se encuentra vencida.");
        }

        TipoMiembro tipoMiembro = DeterminarTipoMiembro(
            usuarioSolicitante.Matricula,
            usuarioSolicitante.NumeroEmpleado);

        var politica = await _politicaPrestamoRepository.ObtenerPorTipoMiembroAsync(
            tipoMiembro,
            cancellationToken);

        if (politica is null)
            return ApplicationResult<int>.Failure("No existe una política de préstamo configurada para el usuario.");

        var resultadoUtilizarReserva = reservaTemporal.Utilizar();

        if (!resultadoUtilizarReserva.IsSuccess)
            return ApplicationResult<int>.Failure(resultadoUtilizarReserva.Error!);

        var resultadoMarcarPrestado = ejemplar.MarcarComoPrestado();

        if (!resultadoMarcarPrestado.IsSuccess)
            return ApplicationResult<int>.Failure(resultadoMarcarPrestado.Error!);

        var resultadoCompletarSolicitud = solicitud.Completar();

        if (!resultadoCompletarSolicitud.IsSuccess)
            return ApplicationResult<int>.Failure(resultadoCompletarSolicitud.Error!);

        DateTime fechaLimiteDevolucion = DateTime.UtcNow.AddDays(
            politica.DiasDuracionPrestamo);

        var prestamo = new Prestamo(
            solicitud.UsuarioId,
            solicitud.EjemplarId,
            command.UsuarioBibliotecarioId,
            fechaLimiteDevolucion,
            solicitud.Id);

        await _prestamoRepository.AgregarAsync(
            prestamo,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(prestamo.Id);
    }

    private static TipoMiembro DeterminarTipoMiembro(
        string? matricula,
        string? numeroEmpleado)
    {
        if (!string.IsNullOrWhiteSpace(numeroEmpleado))
            return TipoMiembro.Docente;

        return TipoMiembro.Estudiante;
    }
}