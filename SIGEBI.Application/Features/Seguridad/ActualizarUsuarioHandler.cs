using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Application.Abstractions.Auditoria;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class ActualizarUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarUsuarioCommand command,
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

        string nombreAnterior = usuario.NombreCompleto;
        string? matriculaAnterior = usuario.Matricula;
        string? numeroEmpleadoAnterior = usuario.NumeroEmpleado;

        usuario.ActualizarDatos(
            command.NombreCompleto,
            command.Matricula,
            command.NumeroEmpleado);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Seguridad",
            accion: "Actualizar usuario",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Usuario",
            entidadAfectadaId: usuario.Id,
            detalle:
                $"Se actualizó el usuario {usuario.Correo}. " +
                $"Nombre anterior: {nombreAnterior}; nombre nuevo: {usuario.NombreCompleto}. " +
                $"Matrícula anterior: {matriculaAnterior ?? "N/A"}; matrícula nueva: {usuario.Matricula ?? "N/A"}. " +
                $"Número empleado anterior: {numeroEmpleadoAnterior ?? "N/A"}; número empleado nuevo: {usuario.NumeroEmpleado ?? "N/A"}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarUsuarioCommand command)
    {
        if (command.UsuarioId <= 0)
            return ApplicationResult.Failure("El usuario es obligatorio.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.NombreCompleto))
            return ApplicationResult.Failure("El nombre completo es obligatorio.");

        return ApplicationResult.Success();
    }
}