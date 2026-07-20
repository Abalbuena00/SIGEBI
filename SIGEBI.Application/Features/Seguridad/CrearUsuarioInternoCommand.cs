namespace SIGEBI.Application.Features.Seguridad;

public sealed class CrearUsuarioInternoCommand
{
    public int UsuarioResponsableId { get; init; }

    public string NombreCompleto { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string NumeroEmpleado { get; init; } = string.Empty;

    public int RolId { get; init; }
}