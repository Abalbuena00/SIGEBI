using SIGEBI.Application.Abstractions.Seguridad;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class RegistrarUsuarioExternoHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarUsuarioExternoHandler(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IPasswordHashService passwordHashService,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _passwordHashService = passwordHashService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        RegistrarUsuarioExternoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return ApplicationResult<int>.Failure(validacion.Error!);

        bool existeCorreo = await _usuarioRepository.ExisteCorreoAsync(
            command.Correo,
            cancellationToken);

        if (existeCorreo)
            return ApplicationResult<int>.Failure("Ya existe un usuario con el mismo correo.");

        string nombreRol = ObtenerNombreRol(command.TipoMiembro);

        var rol = await _rolRepository.ObtenerPorNombreAsync(
            nombreRol,
            cancellationToken);

        if (rol is null)
            return ApplicationResult<int>.Failure("El rol requerido para el autorregistro no existe.");

        if (!rol.Activo)
            return ApplicationResult<int>.Failure("El rol requerido para el autorregistro no está activo.");

        string passwordHash = _passwordHashService.HashPassword(command.Password);

        string? matricula = command.TipoMiembro == TipoMiembro.Estudiante
            ? command.Matricula
            : null;

        string? numeroEmpleado = command.TipoMiembro == TipoMiembro.Docente
            ? command.NumeroEmpleado
            : null;

        var usuario = new Usuario(
            command.NombreCompleto,
            command.Correo,
            passwordHash,
            matricula,
            numeroEmpleado);

        usuario.AgregarRol(new UsuarioRol(rol.Id));

        await _usuarioRepository.AgregarAsync(
            usuario,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(usuario.Id);
    }

    private static ApplicationResult ValidarCommand(
        RegistrarUsuarioExternoCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.NombreCompleto))
            return ApplicationResult.Failure("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Correo))
            return ApplicationResult.Failure("El correo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Password))
            return ApplicationResult.Failure("La contraseña es obligatoria.");

        if (!Enum.IsDefined(typeof(TipoMiembro), command.TipoMiembro))
            return ApplicationResult.Failure("El tipo de miembro no es válido.");

        if (command.TipoMiembro == TipoMiembro.Estudiante &&
            string.IsNullOrWhiteSpace(command.Matricula))
        {
            return ApplicationResult.Failure("La matrícula es obligatoria para estudiantes.");
        }

        if (command.TipoMiembro == TipoMiembro.Docente &&
            string.IsNullOrWhiteSpace(command.NumeroEmpleado))
        {
            return ApplicationResult.Failure("El número de empleado es obligatorio para docentes.");
        }

        return ApplicationResult.Success();
    }

    private static string ObtenerNombreRol(TipoMiembro tipoMiembro)
    {
        return tipoMiembro switch
        {
            TipoMiembro.Estudiante => "Estudiante",
            TipoMiembro.Docente => "Docente",
            _ => throw new InvalidOperationException("Tipo de miembro no soportado.")
        };
    }
}