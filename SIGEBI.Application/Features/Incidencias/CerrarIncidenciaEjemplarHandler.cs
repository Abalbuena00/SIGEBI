using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Incidencias;

public sealed class CerrarIncidenciaEjemplarHandler
{
    private readonly IIncidenciaEjemplarRepository _incidenciaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CerrarIncidenciaEjemplarHandler(
        IIncidenciaEjemplarRepository incidenciaRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _incidenciaRepository = incidenciaRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        CerrarIncidenciaEjemplarCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.IncidenciaId <= 0)
            return ApplicationResult.Failure("La incidencia es obligatoria.");

        if (command.UsuarioCierreId <= 0)
            return ApplicationResult.Failure("El usuario que cierra la incidencia es obligatorio.");

        var incidencia = await _incidenciaRepository.ObtenerPorIdAsync(
            command.IncidenciaId,
            cancellationToken);

        if (incidencia is null)
            return ApplicationResult.Failure("La incidencia no fue encontrada.");

        var usuarioCierre = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioCierreId,
            cancellationToken);

        if (usuarioCierre is null)
            return ApplicationResult.Failure("El usuario que cierra la incidencia no fue encontrado.");

        if (usuarioCierre.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario que cierra la incidencia no se encuentra activo.");

        var resultadoCierre = incidencia.Cerrar(command.UsuarioCierreId);

        if (!resultadoCierre.IsSuccess)
            return ApplicationResult.Failure(resultadoCierre.Error!);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioCierreId,
            modulo: "Incidencias",
            accion: "Cerrar incidencia de ejemplar",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "IncidenciaEjemplar",
            entidadAfectadaId: incidencia.Id,
            detalle:
                $"Se cerró la incidencia {incidencia.Id} del ejemplar " +
                $"{incidencia.EjemplarId}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}
