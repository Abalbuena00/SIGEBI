using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class RegistrarUsuarioExternoCommand
{
    public string NombreCompleto { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public TipoMiembro TipoMiembro { get; init; }

    public string? Matricula { get; init; }

    public string? NumeroEmpleado { get; init; }
}