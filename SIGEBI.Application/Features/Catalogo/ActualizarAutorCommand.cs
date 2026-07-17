namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarAutorCommand
{
    public int AutorId { get; init; }

    public string Nombre { get; init; } = string.Empty;
}