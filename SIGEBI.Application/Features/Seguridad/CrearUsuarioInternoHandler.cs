using SIGEBI.Application.Abstractions.Seguridad;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Application.Abstractions.Auditoria;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class CrearUsuarioInternoHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CrearUsuarioInternoHandler(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IPasswordHashService passwordHashService,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _passwordHashService = passwordHashService;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearUsuarioInternoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return ApplicationResult<int>.Failure(validacion.Error!);

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult<int>.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El usuario responsable no se encuentra activo.");

        bool existeCorreo = await _usuarioRepository.ExisteCorreoAsync(
            command.Correo,
            cancellationToken);

        if (existeCorreo)
            return ApplicationResult<int>.Failure("Ya existe un usuario con el mismo correo.");

        var rol = await _rolRepository.ObtenerPorIdAsync(
            command.RolId,
            cancellationToken);

        if (rol is null)
            return ApplicationResult<int>.Failure("El rol no fue encontrado.");

        if (!rol.Activo)
            return ApplicationResult<int>.Failure("El rol no está activo.");

        if (EsRolExterno(rol.Nombre))
            return ApplicationResult<int>.Failure("Este caso de uso solo permite crear usuarios internos.");

        string passwordHash = _passwordHashService.HashPassword(command.Password);

        var usuario = new Usuario(
            command.NombreCompleto,
            command.Correo,
            passwordHash,
            matricula: null,
            numeroEmpleado: command.NumeroEmpleado);

        usuario.AgregarRol(new UsuarioRol(rol.Id));

        await _usuarioRepository.AgregarAsync(
            usuario,
            cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Seguridad",
            accion: "Crear usuario interno",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Usuario",
            entidadAfectadaId: null,
            detalle: $"Se creó el usuario interno {usuario.Correo} con rol {rol.Nombre}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(usuario.Id);
    }

    private static ApplicationResult ValidarCommand(
        CrearUsuarioInternoCommand command)
    {
        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.NombreCompleto))
            return ApplicationResult.Failure("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Correo))
            return ApplicationResult.Failure("El correo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Password))
            return ApplicationResult.Failure("La contraseña es obligatoria.");

        if (string.IsNullOrWhiteSpace(command.NumeroEmpleado))
            return ApplicationResult.Failure("El número de empleado es obligatorio.");

        if (command.RolId <= 0)
            return ApplicationResult.Failure("El rol es obligatorio.");

        return ApplicationResult.Success();
    }

    private static bool EsRolExterno(string nombreRol)
    {
        return nombreRol.Equals("Estudiante", StringComparison.OrdinalIgnoreCase) ||
               nombreRol.Equals("Docente", StringComparison.OrdinalIgnoreCase);
    }
}