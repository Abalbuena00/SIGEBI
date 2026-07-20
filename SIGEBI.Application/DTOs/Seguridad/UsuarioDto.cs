namespace SIGEBI.Application.DTOs.Seguridad;

public sealed class UsuarioDto
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string? Matricula { get; set; }

    public string? NumeroEmpleado { get; set; }

    public int Estado { get; set; }

    public string EstadoDescripcion { get; set; } = string.Empty;

    public IReadOnlyList<string> Roles { get; set; } = [];
}