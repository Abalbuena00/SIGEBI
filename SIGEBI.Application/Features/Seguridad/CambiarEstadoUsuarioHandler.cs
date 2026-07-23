using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Application.Abstractions.Auditoria;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class CambiarEstadoUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CambiarEstadoUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        CambiarEstadoUsuarioCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioId,
            cancellationToken);

        if (usuario is null)
            return ApplicationResult.Failure("El usuario no fue encontrado.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        if (usuario.Id == usuarioResponsable.Id)
            return ApplicationResult.Failure("Un usuario no puede cambiar su propio estado.");

        EstadoUsuario estadoAnterior = usuario.Estado;

        usuario.CambiarEstado(command.NuevoEstado);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Seguridad",
            accion: "Cambiar estado de usuario",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Usuario",
            entidadAfectadaId: usuario.Id,
            detalle:
                $"Se cambió el estado del usuario {usuario.Correo}. " +
                $"Estado anterior: {estadoAnterior}; estado nuevo: {usuario.Estado}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        CambiarEstadoUsuarioCommand command)
    {
        if (command.UsuarioId <= 0)
            return ApplicationResult.Failure("El usuario es obligatorio.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (!Enum.IsDefined(typeof(EstadoUsuario), command.NuevoEstado))
            return ApplicationResult.Failure("El estado indicado no es válido.");

        return ApplicationResult.Success();
    }
}