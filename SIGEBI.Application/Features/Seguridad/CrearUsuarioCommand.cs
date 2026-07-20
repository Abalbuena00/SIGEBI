namespace SIGEBI.Application.Features.Seguridad;

public sealed class CrearUsuarioCommand
{
    public string NombreCompleto { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public string PasswordHash { get; init; } = string.Empty;

    public string? Matricula { get; init; }

    public string? NumeroEmpleado { get; init; }
}