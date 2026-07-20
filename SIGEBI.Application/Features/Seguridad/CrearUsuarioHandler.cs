using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class CrearUsuarioHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearUsuarioHandler(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearUsuarioCommand command,
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

        var usuario = new Usuario(
            command.NombreCompleto,
            command.Correo,
            command.PasswordHash,
            command.Matricula,
            command.NumeroEmpleado);

        await _usuarioRepository.AgregarAsync(
            usuario,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(usuario.Id);
    }

    private static ApplicationResult ValidarCommand(
        CrearUsuarioCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.NombreCompleto))
            return ApplicationResult.Failure("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Correo))
            return ApplicationResult.Failure("El correo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.PasswordHash))
            return ApplicationResult.Failure("La contraseña protegida es obligatoria.");

        return ApplicationResult.Success();
    }
}