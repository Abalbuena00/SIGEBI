namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarCategoriaCommand
{
    public int CategoriaId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string? Descripcion { get; init; }
}
