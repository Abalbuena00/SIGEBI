using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Configuracion;

public sealed class ActualizarParametroSistemaHandler
{
    private readonly IParametroSistemaRepository _parametroSistemaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarParametroSistemaHandler(
        IParametroSistemaRepository parametroSistemaRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _parametroSistemaRepository = parametroSistemaRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarParametroSistemaCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var parametro = await _parametroSistemaRepository.ObtenerPorIdAsync(
            command.ParametroSistemaId,
            cancellationToken);

        if (parametro is null)
            return ApplicationResult.Failure("El parámetro del sistema no fue encontrado.");

        string valorAnterior = parametro.Valor;
        string? descripcionAnterior = parametro.Descripcion;

        parametro.ActualizarValor(
            command.Valor,
            command.Descripcion);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Configuración",
            accion: "Actualizar parámetro del sistema",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "ParametroSistema",
            entidadAfectadaId: parametro.Id,
            detalle:
                $"Se actualizó el parámetro {parametro.Clave}. " +
                $"Valor anterior: {valorAnterior}; valor nuevo: {parametro.Valor}. " +
                $"Descripción anterior: {descripcionAnterior ?? "sin descripción"}; " +
                $"descripción nueva: {parametro.Descripcion ?? "sin descripción"}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarParametroSistemaCommand command)
    {
        if (command.ParametroSistemaId <= 0)
            return ApplicationResult.Failure("El parámetro del sistema es obligatorio.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Valor))
            return ApplicationResult.Failure("El valor del parámetro es obligatorio.");

        return ApplicationResult.Success();
    }
}
