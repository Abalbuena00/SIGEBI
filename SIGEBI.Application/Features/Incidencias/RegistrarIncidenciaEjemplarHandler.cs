using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Incidencias;

public sealed class RegistrarIncidenciaEjemplarHandler
{
    private readonly IIncidenciaEjemplarRepository _incidenciaRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarIncidenciaEjemplarHandler(
        IIncidenciaEjemplarRepository incidenciaRepository,
        IEjemplarRepository ejemplarRepository,
        IPrestamoRepository prestamoRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _incidenciaRepository = incidenciaRepository;
        _ejemplarRepository = ejemplarRepository;
        _prestamoRepository = prestamoRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        RegistrarIncidenciaEjemplarCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            command.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult.Failure("El ejemplar no fue encontrado.");

        var usuarioReporta = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioReportaId,
            cancellationToken);

        if (usuarioReporta is null)
            return ApplicationResult.Failure("El usuario que reporta no fue encontrado.");

        if (usuarioReporta.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario que reporta no se encuentra activo.");

        Prestamo? prestamo = null;

        if (command.PrestamoId.HasValue)
        {
            prestamo = await _prestamoRepository.ObtenerPorIdAsync(
                command.PrestamoId.Value,
                cancellationToken);

            if (prestamo is null)
                return ApplicationResult.Failure("El préstamo no fue encontrado.");

            if (prestamo.EjemplarId != command.EjemplarId)
            {
                return ApplicationResult.Failure(
                    "El préstamo indicado no corresponde al ejemplar.");
            }
        }


        bool dejaFueraDeServicio =
            command.Tipo == TipoIncidenciaEjemplar.Dano ||
            command.Tipo == TipoIncidenciaEjemplar.Perdida;


        if (dejaFueraDeServicio &&
            prestamo is null &&
            ejemplar.Estado == EstadoEjemplar.Prestado)
        {
            prestamo = await _prestamoRepository.ObtenerActivoPorEjemplarAsync(
                command.EjemplarId,
                cancellationToken);

            if (prestamo is null)
            {
                return ApplicationResult.Failure(
                    "El ejemplar está prestado, pero no se encontró un préstamo activo asociado.");
            }
        }

        var incidencia = new IncidenciaEjemplar(
        command.EjemplarId,
        command.UsuarioReportaId,
        command.Tipo,
        command.Descripcion,
        prestamo?.Id);

        await _incidenciaRepository.AgregarAsync(
            incidencia,
            cancellationToken);

        if (dejaFueraDeServicio)
        {
            MarcarEjemplarFueraDeServicio(ejemplar, command);

            if (prestamo is not null &&
                (prestamo.Estado == EstadoPrestamo.Activo ||
                 prestamo.Estado == EstadoPrestamo.Vencido))
            {
                var resultadoCierre = prestamo.CerrarConIncidencia(DateTime.UtcNow);

                if (!resultadoCierre.IsSuccess)
                    return ApplicationResult.Failure(resultadoCierre.Error!);
            }
        }

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioReportaId,
            modulo: "Incidencias",
            accion: "Registrar incidencia de ejemplar",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "IncidenciaEjemplar",
            entidadAfectadaId: null,
            detalle:
                $"Se registró una incidencia de tipo {command.Tipo} para el ejemplar " +
                $"{command.EjemplarId}. Préstamo asociado: " +
                $"{prestamo?.Id.ToString() ?? "ninguno"}." +
                $"Descripción: {command.Descripcion.Trim()}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static void MarcarEjemplarFueraDeServicio(
        Ejemplar ejemplar,
        RegistrarIncidenciaEjemplarCommand command)
    {
        EstadoEjemplar estadoAnterior = ejemplar.Estado;
        string motivo = $"{command.Tipo}: {command.Descripcion.Trim()}";

        ejemplar.MarcarFueraDeServicio(motivo);

        var historial = new HistorialEstadoEjemplar(
            ejemplar.Id,
            estadoAnterior,
            ejemplar.Estado,
            command.UsuarioReportaId,
            motivo);

        ejemplar.RegistrarHistorialEstado(historial);
    }

    private static ApplicationResult ValidarCommand(
        RegistrarIncidenciaEjemplarCommand command)
    {
        if (command.EjemplarId <= 0)
            return ApplicationResult.Failure("El ejemplar es obligatorio.");

        if (command.UsuarioReportaId <= 0)
            return ApplicationResult.Failure("El usuario que reporta es obligatorio.");

        if (!Enum.IsDefined(typeof(TipoIncidenciaEjemplar), command.Tipo))
            return ApplicationResult.Failure("El tipo de incidencia indicado no es válido.");

        if (string.IsNullOrWhiteSpace(command.Descripcion))
            return ApplicationResult.Failure("La descripción de la incidencia es obligatoria.");

        if (command.PrestamoId.HasValue && command.PrestamoId.Value <= 0)
            return ApplicationResult.Failure("El préstamo indicado no es válido.");

        return ApplicationResult.Success();
    }
}
