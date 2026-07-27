using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class AsignarRolUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public AsignarRolUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        AsignarRolUsuarioCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var validacion = ValidarCommand(command);
        if (!validacion.IsSuccess)
            return validacion;

        var responsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId, cancellationToken);
        if (responsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");
        if (responsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var usuario = await _usuarioRepository.ObtenerConRolesPorIdAsync(
            command.UsuarioId, cancellationToken);
        if (usuario is null)
            return ApplicationResult.Failure("El usuario no fue encontrado.");
        if (usuario.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario no se encuentra activo.");

        var rol = await _rolRepository.ObtenerPorIdAsync(command.RolId, cancellationToken);
        if (rol is null)
            return ApplicationResult.Failure("El rol no fue encontrado.");
        if (!rol.Activo)
            return ApplicationResult.Failure("El rol no se encuentra activo.");
        if (usuario.Roles.Any(usuarioRol => usuarioRol.RolId == rol.Id))
            return ApplicationResult.Failure("El usuario ya tiene asignado el rol indicado.");

        usuario.AgregarRol(new UsuarioRol(usuario.Id, rol.Id));

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Seguridad",
            accion: "Asignar rol a usuario",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Usuario",
            entidadAfectadaId: usuario.Id,
            detalle: $"Se asignó el rol {rol.Nombre} al usuario {usuario.Correo}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(AsignarRolUsuarioCommand command)
    {
        if (command.UsuarioId <= 0)
            return ApplicationResult.Failure("El usuario es obligatorio.");
        if (command.RolId <= 0)
            return ApplicationResult.Failure("El rol es obligatorio.");
        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        return ApplicationResult.Success();
    }
}
