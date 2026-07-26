namespace SIGEBI.Application.Features.Configuracion;

public sealed class CrearParametroSistemaCommand
{
    public int UsuarioResponsableId { get; init; }

    public string Clave { get; init; } = string.Empty;

    public string Valor { get; init; } = string.Empty;

    public string? Descripcion { get; init; }
}
