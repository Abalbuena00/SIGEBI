using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Configuracion;

public sealed class CrearParametroSistemaHandler
{
    private readonly IParametroSistemaRepository _parametroSistemaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CrearParametroSistemaHandler(
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
        CrearParametroSistemaCommand command,
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

        var parametroExistente = await _parametroSistemaRepository.ObtenerPorClaveAsync(
            command.Clave,
            cancellationToken);

        if (parametroExistente is not null)
        {
            return ApplicationResult.Failure(
                "Ya existe un parámetro activo con la misma clave.");
        }

        var parametro = new ParametroSistema(
            command.Clave,
            command.Valor,
            command.Descripcion);

        await _parametroSistemaRepository.AgregarAsync(
            parametro,
            cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Configuración",
            accion: "Crear parámetro del sistema",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "ParametroSistema",
            entidadAfectadaId: null,
            detalle:
                $"Se creó el parámetro del sistema con clave {parametro.Clave}. " +
                $"Valor: {parametro.Valor}. Descripción: " +
                $"{parametro.Descripcion ?? "sin descripción"}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        CrearParametroSistemaCommand command)
    {
        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Clave))
            return ApplicationResult.Failure("La clave del parámetro es obligatoria.");

        if (string.IsNullOrWhiteSpace(command.Valor))
            return ApplicationResult.Failure("El valor del parámetro es obligatorio.");

        return ApplicationResult.Success();
    }
}
