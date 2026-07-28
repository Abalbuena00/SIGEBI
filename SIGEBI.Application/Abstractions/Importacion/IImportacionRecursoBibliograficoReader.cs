namespace SIGEBI.Application.Abstractions.Importacion;

public interface IImportacionRecursoBibliograficoReader
{
    Task<IReadOnlyList<FilaImportacionRecursoBibliografico>> LeerAsync(
        string nombreArchivo,
        string contentType,
        Stream contenido,
        CancellationToken cancellationToken = default);
}
