using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Application.Abstractions.Auditoria;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class RegistrarDevolucionPrestamoHandler
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPoliticaPrestamoRepository _politicaPrestamoRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarDevolucionPrestamoHandler(
        IPrestamoRepository prestamoRepository,
        IEjemplarRepository ejemplarRepository,
        IUsuarioRepository usuarioRepository,
        IPoliticaPrestamoRepository politicaPrestamoRepository,
        IPenalizacionRepository penalizacionRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _prestamoRepository = prestamoRepository;
        _ejemplarRepository = ejemplarRepository;
        _usuarioRepository = usuarioRepository;
        _politicaPrestamoRepository = politicaPrestamoRepository;
        _penalizacionRepository = penalizacionRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        RegistrarDevolucionPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.PrestamoId <= 0)
            return ApplicationResult.Failure("El préstamo es obligatorio.");

        if (command.UsuarioBibliotecarioId <= 0)
            return ApplicationResult.Failure("El bibliotecario responsable es obligatorio.");

        var prestamo = await _prestamoRepository.ObtenerPorIdAsync(
            command.PrestamoId,
            cancellationToken);

        if (prestamo is null)
            return ApplicationResult.Failure("El préstamo no fue encontrado.");

        if (prestamo.Estado != EstadoPrestamo.Activo &&
            prestamo.Estado != EstadoPrestamo.Vencido)
        {
            return ApplicationResult.Failure("El préstamo no se encuentra activo para devolución.");
        }

        var bibliotecario = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioBibliotecarioId,
            cancellationToken);

        if (bibliotecario is null)
            return ApplicationResult.Failure("El bibliotecario responsable no fue encontrado.");

        if (bibliotecario.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El bibliotecario responsable no se encuentra activo.");

        var usuarioSolicitante = await _usuarioRepository.ObtenerPorIdAsync(
            prestamo.UsuarioId,
            cancellationToken);

        if (usuarioSolicitante is null)
            return ApplicationResult.Failure("El usuario del préstamo no fue encontrado.");

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            prestamo.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult.Failure("El ejemplar asociado al préstamo no fue encontrado.");

        DateTime fechaDevolucion = command.FechaDevolucion ?? DateTime.UtcNow;

        var devolucion = new Devolucion(
            prestamo.Id,
            command.UsuarioBibliotecarioId,
            prestamo.FechaLimiteDevolucion,
            fechaDevolucion,
            command.Observacion);

        var resultadoPrestamo = prestamo.RegistrarDevolucion(fechaDevolucion);

        if (!resultadoPrestamo.IsSuccess)
            return ApplicationResult.Failure(resultadoPrestamo.Error!);

        var resultadoEjemplar = ejemplar.RegistrarDevolucion();

        if (!resultadoEjemplar.IsSuccess)
            return ApplicationResult.Failure(resultadoEjemplar.Error!);

        prestamo.AgregarDevolucion(devolucion);

        await CrearPenalizacionSiAplicaAsync(
            prestamo,
            usuarioSolicitante.Matricula,
            usuarioSolicitante.NumeroEmpleado,
            devolucion,
            cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioBibliotecarioId,
            modulo: "Devoluciones",
            accion: "Registrar devolución de préstamo",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Prestamo",
            entidadAfectadaId: prestamo.Id,
            detalle:
                $"Se registró la devolución del préstamo {prestamo.Id}. " +
                $"Usuario: {prestamo.UsuarioId}. Ejemplar: {prestamo.EjemplarId}. " +
                $"Fecha devolución: {fechaDevolucion:yyyy-MM-dd HH:mm:ss}. " +
                $"Devolución tardía: {(devolucion.FueTardia ? "Sí" : "No")}. " +
                $"Días de retraso: {devolucion.DiasRetraso}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private async Task CrearPenalizacionSiAplicaAsync(
        Prestamo prestamo,
        string? matricula,
        string? numeroEmpleado,
        Devolucion devolucion,
        CancellationToken cancellationToken)
    {
        if (!devolucion.FueTardia)
            return;

        TipoMiembro tipoMiembro = DeterminarTipoMiembro(
            matricula,
            numeroEmpleado);

        var politica = await _politicaPrestamoRepository.ObtenerPorTipoMiembroAsync(
            tipoMiembro,
            cancellationToken);

        if (politica is null)
            return;

        if (!politica.PenalizaRetraso)
            return;

        int diasSuspension = devolucion.DiasRetraso *
                             politica.DiasSuspensionPorDiaRetraso;

        if (diasSuspension <= 0)
            return;

        var penalizacion = new Penalizacion(
            prestamo.UsuarioId,
            diasSuspension,
            prestamo.Id);

        await _penalizacionRepository.AgregarAsync(
            penalizacion,
            cancellationToken);
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