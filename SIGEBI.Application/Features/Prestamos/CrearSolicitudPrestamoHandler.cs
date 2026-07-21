using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.Exceptions;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class CrearSolicitudPrestamoHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IPoliticaPrestamoRepository _politicaPrestamoRepository;
    private readonly ISolicitudPrestamoRepository _solicitudPrestamoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearSolicitudPrestamoHandler(
        IUsuarioRepository usuarioRepository,
        IEjemplarRepository ejemplarRepository,
        IPenalizacionRepository penalizacionRepository,
        IPrestamoRepository prestamoRepository,
        IPoliticaPrestamoRepository politicaPrestamoRepository,
        ISolicitudPrestamoRepository solicitudPrestamoRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _ejemplarRepository = ejemplarRepository;
        _penalizacionRepository = penalizacionRepository;
        _prestamoRepository = prestamoRepository;
        _politicaPrestamoRepository = politicaPrestamoRepository;
        _solicitudPrestamoRepository = solicitudPrestamoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearSolicitudPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioId <= 0)
            return ApplicationResult<int>.Failure("El usuario solicitante es obligatorio.");

        if (command.EjemplarId <= 0)
            return ApplicationResult<int>.Failure("El ejemplar es obligatorio.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult<int>.Failure("El usuario solicitante no fue encontrado.");

        if (usuario.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El usuario no se encuentra activo.");

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            command.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult<int>.Failure("El ejemplar no fue encontrado.");

        if (ejemplar.Estado != EstadoEjemplar.Disponible)
            return ApplicationResult<int>.Failure("El ejemplar no se encuentra disponible.");

        bool tienePenalizacionActiva = await _penalizacionRepository.TienePenalizacionActivaAsync(
            command.UsuarioId,
            cancellationToken);

        if (tienePenalizacionActiva)
            return ApplicationResult<int>.Failure("El usuario posee una penalización activa.");

        TipoMiembro tipoMiembro = DeterminarTipoMiembro(usuario.Matricula, usuario.NumeroEmpleado);

        var politica = await _politicaPrestamoRepository.ObtenerPorTipoMiembroAsync(
            tipoMiembro,
            cancellationToken);

        if (politica is null)
            return ApplicationResult<int>.Failure("No existe una política de préstamo configurada para el usuario.");

        int prestamosActivos = await _prestamoRepository.ContarActivosPorUsuarioAsync(
            command.UsuarioId,
            cancellationToken);

        if (prestamosActivos >= politica.MaximoPrestamosActivos)
            return ApplicationResult<int>.Failure("El usuario alcanzó el máximo de préstamos activos permitidos.");

        var solicitudesUsuario = await _solicitudPrestamoRepository.ObtenerPorUsuarioAsync(
            command.UsuarioId,
            cancellationToken);

        bool tieneSolicitudActivaParaEjemplar = solicitudesUsuario.Any(solicitud =>
            solicitud.EjemplarId == command.EjemplarId &&
            (solicitud.Estado == EstadoSolicitudPrestamo.Pendiente ||
             solicitud.Estado == EstadoSolicitudPrestamo.AprobadaPendienteRetiro));

        if (tieneSolicitudActivaParaEjemplar)
            return ApplicationResult<int>.Failure("El usuario ya tiene una solicitud activa para este ejemplar.");

        var resultadoReservaEjemplar = ejemplar.Reservar();

        if (!resultadoReservaEjemplar.IsSuccess)
            return ApplicationResult<int>.Failure(resultadoReservaEjemplar.Error!);

        var solicitudPrestamo = new SolicitudPrestamo(
            command.UsuarioId,
            command.EjemplarId,
            politica.HorasReservaTemporal);

        await _solicitudPrestamoRepository.AgregarAsync(
            solicitudPrestamo,
            cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return ApplicationResult<int>.Failure(
                "El ejemplar ya fue reservado por otro usuario. Intente seleccionar otro ejemplar disponible.");
        }

        return ApplicationResult<int>.Success(solicitudPrestamo.Id);
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