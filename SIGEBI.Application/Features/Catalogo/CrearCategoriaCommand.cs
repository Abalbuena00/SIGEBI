namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearCategoriaCommand
{
    public int UsuarioResponsableId { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string? Descripcion { get; init; }
}
