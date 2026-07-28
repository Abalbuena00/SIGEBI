using SIGEBI.Application.Abstractions.Archivos;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
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
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarImagenRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IUsuarioRepository usuarioRepository,
        IFileStorageService fileStorageService,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _usuarioRepository = usuarioRepository;
        _fileStorageService = fileStorageService;
        _auditoriaService = auditoriaService;
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

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var recurso = await _recursoBibliograficoRepository.ObtenerPorIdAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        string? imagenAnterior = command.TipoImagen == TipoImagenRecursoBibliografico.Portada
            ? recurso.ImagenPortadaNombreArchivo ?? recurso.ImagenPortadaUrl
            : recurso.ImagenContraportadaNombreArchivo ?? recurso.ImagenContraportadaUrl;

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

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Actualizar imagen de recurso bibliogr\u00E1fico",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: recurso.Id,
            detalle:
                $"Recurso: {recurso.CodigoInterno} - {recurso.Titulo}. " +
                $"Tipo de imagen: {command.TipoImagen}. " +
                $"Imagen anterior: {imagenAnterior ?? "N/A"}. " +
                $"Nombre nuevo: {command.NombreArchivo.Trim()}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarImagenRecursoBibliograficoCommand command)
    {
        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

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
