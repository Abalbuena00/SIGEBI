using SIGEBI.Application.Abstractions.Notificaciones;
using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class GenerarRecordatoriosPrestamosPorVencerHandler
{
    private const string EntidadReferencia = "Prestamo";
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly INotificacionRepository _notificacionRepository;
    private readonly INotificacionService _notificacionService;
    private readonly IUnitOfWork _unitOfWork;

    public GenerarRecordatoriosPrestamosPorVencerHandler(
        IPrestamoRepository prestamoRepository,
        INotificacionRepository notificacionRepository,
        INotificacionService notificacionService,
        IUnitOfWork unitOfWork)
    {
        _prestamoRepository = prestamoRepository;
        _notificacionRepository = notificacionRepository;
        _notificacionService = notificacionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ResultadoRecordatoriosPrestamosDto>> HandleAsync(
        GenerarRecordatoriosPrestamosPorVencerCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.DiasAnticipacion <= 0)
            return ApplicationResult<ResultadoRecordatoriosPrestamosDto>.Failure(
                "Los d\u00EDas de anticipaci\u00F3n deben ser mayores que cero.");

        var fechaReferencia = command.FechaReferencia ?? DateTime.UtcNow;
        DateTime fechaLimite;

        try
        {
            fechaLimite = fechaReferencia.AddDays(command.DiasAnticipacion);
        }
        catch (ArgumentOutOfRangeException)
        {
            return ApplicationResult<ResultadoRecordatoriosPrestamosDto>.Failure(
                "El per\u00EDodo de anticipaci\u00F3n excede el rango de fechas permitido.");
        }

        var prestamos = await _prestamoRepository.ObtenerProximosAVencerAsync(
            fechaReferencia, fechaLimite, cancellationToken);
        var notificacionesCreadas = 0;

        foreach (var prestamo in prestamos)
        {
            var existe = await _notificacionRepository.ExistePorReferenciaAsync(
                prestamo.UsuarioId,
                TipoNotificacion.ProximoVencimiento,
                EntidadReferencia,
                prestamo.Id,
                cancellationToken);

            if (existe)
                continue;

            await _notificacionService.CrearAsync(
                prestamo.UsuarioId,
                TipoNotificacion.ProximoVencimiento,
                "Pr\u00E9stamo pr\u00F3ximo a vencer",
                $"Su pr\u00E9stamo vence el {prestamo.FechaLimiteDevolucion:dd/MM/yyyy}. Favor realizar la devoluci\u00F3n a tiempo para evitar penalizaciones.",
                EntidadReferencia,
                prestamo.Id,
                cancellationToken);

            notificacionesCreadas++;
        }

        if (notificacionesCreadas > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resultado = new ResultadoRecordatoriosPrestamosDto
        {
            PrestamosEvaluados = prestamos.Count,
            NotificacionesCreadas = notificacionesCreadas,
            FechaReferencia = fechaReferencia,
            FechaLimiteEvaluada = fechaLimite
        };

        return ApplicationResult<ResultadoRecordatoriosPrestamosDto>.Success(resultado);
    }
}
