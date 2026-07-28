namespace SIGEBI.Application.DTOs.Catalogo;

public sealed class DetalleImportacionRecursoBibliograficoDto
{
    public int NumeroFila { get; init; }
    public bool Importada { get; init; }
    public string CodigoInternoRecurso { get; init; } = string.Empty;
    public string? CodigoInternoEjemplar { get; init; }
    public string Mensaje { get; init; } = string.Empty;
}
