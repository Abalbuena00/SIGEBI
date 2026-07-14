using System;
namespace SIGEBI.Application.Abstractions.Archivos;

public interface IFileStorageService
{
    Task<string> GuardarImagenRecursoBibliograficoAsync(
        int recursoBibliograficoId,
        TipoImagenRecursoBibliografico tipoImagen,
        string nombreArchivo,
        string contentType,
        Stream contenido,
        CancellationToken cancellationToken = default);
}