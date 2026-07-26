namespace SIGEBI.Application.DTOs.Configuracion;

public sealed class ParametroSistemaDto
{
    public int Id { get; set; }

    public string Clave { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}
