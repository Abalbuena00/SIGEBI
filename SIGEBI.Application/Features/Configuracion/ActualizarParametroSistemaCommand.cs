namespace SIGEBI.Application.Features.Configuracion;

public sealed class ActualizarParametroSistemaCommand
{
    public int ParametroSistemaId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string Valor { get; init; } = string.Empty;

    public string? Descripcion { get; init; }
}
