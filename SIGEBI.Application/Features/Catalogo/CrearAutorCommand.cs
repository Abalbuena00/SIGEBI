namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearAutorCommand
{
    public int UsuarioResponsableId { get; init; }

    public string Nombre { get; init; } = string.Empty;
}
