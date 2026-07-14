using SIGEBI.Application.Abstractions.Archivos;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarImagenRecursoBibliograficoHandler
{
    private const long TamanoMaximoBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> ContentTypesPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarImagenRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarImagenRecursoBibliograficoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var recurso = await _recursoBibliograficoRepository.ObtenerPorIdAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        string imagenUrl = await _fileStorageService.GuardarImagenRecursoBibliograficoAsync(
            command.RecursoBibliograficoId,
            command.TipoImagen,
            command.NombreArchivo,
            command.ContentType,
            command.Contenido,
            cancellationToken);

        if (command.TipoImagen == TipoImagenRecursoBibliografico.Portada)
        {
            recurso.ActualizarImagenPortada(
                imagenUrl,
                command.NombreArchivo,
                command.ContentType);
        }
        else
        {
            recurso.ActualizarImagenContraportada(
                imagenUrl,
                command.NombreArchivo,
                command.ContentType);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarImagenRecursoBibliograficoCommand command)
    {
        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (!Enum.IsDefined(command.TipoImagen))
            return ApplicationResult.Failure("El tipo de imagen no es válido.");

        if (string.IsNullOrWhiteSpace(command.NombreArchivo))
            return ApplicationResult.Failure("El nombre del archivo es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.ContentType))
            return ApplicationResult.Failure("El tipo de contenido del archivo es obligatorio.");

        if (!ContentTypesPermitidos.Contains(command.ContentType))
            return ApplicationResult.Failure("Solo se permiten imágenes JPG, PNG o WEBP.");

        string extension = Path.GetExtension(command.NombreArchivo);

        if (!ExtensionesPermitidas.Contains(extension))
            return ApplicationResult.Failure("La extensión del archivo no está permitida.");

        if (command.TamanoBytes <= 0)
            return ApplicationResult.Failure("El archivo no puede estar vacío.");

        if (command.TamanoBytes > TamanoMaximoBytes)
            return ApplicationResult.Failure("La imagen no puede superar los 5 MB.");

        if (command.Contenido == Stream.Null)
            return ApplicationResult.Failure("El contenido del archivo es obligatorio.");

        return ApplicationResult.Success();
    }
}