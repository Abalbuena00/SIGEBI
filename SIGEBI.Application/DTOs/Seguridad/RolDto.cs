namespace SIGEBI.Application.DTOs.Seguridad;

public sealed class RolDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
}