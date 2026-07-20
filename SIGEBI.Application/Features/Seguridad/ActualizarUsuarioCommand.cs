namespace SIGEBI.Application.Features.Seguridad;

public sealed class ActualizarUsuarioCommand
{
    public int UsuarioId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string NombreCompleto { get; init; } = string.Empty;

    public string? Matricula { get; init; }

    public string? NumeroEmpleado { get; init; }
}