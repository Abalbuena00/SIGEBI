using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarRecursoBibliograficoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarRecursoBibliograficoCommand command,
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

        if (!string.IsNullOrWhiteSpace(command.Isbn))
        {
            var recursoExistentePorIsbn = await _recursoBibliograficoRepository.ObtenerPorIsbnAsync(
                command.Isbn,
                cancellationToken);

            if (recursoExistentePorIsbn is not null && recursoExistentePorIsbn.Id != recurso.Id)
                return ApplicationResult.Failure("Ya existe otro recurso bibliográfico con el mismo ISBN.");
        }

        string tituloAnterior = recurso.Titulo;
        string? isbnAnterior = recurso.Isbn;
        string? editorialAnterior = recurso.Editorial;
        int? anioAnterior = recurso.AnioPublicacion;
        string? edicionAnterior = recurso.Edicion;

        recurso.ActualizarDatos(
            command.Titulo,
            command.Isbn,
            command.Editorial,
            command.AnioPublicacion,
            command.Edicion);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Catálogo",
            accion: "Actualizar recurso bibliográfico",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: recurso.Id,
            detalle:
                $"Se actualizó el recurso {recurso.CodigoInterno}. " +
                $"Título anterior: {tituloAnterior}; título nuevo: {recurso.Titulo}. " +
                $"ISBN anterior: {isbnAnterior ?? "N/A"}; ISBN nuevo: {recurso.Isbn ?? "N/A"}. " +
                $"Editorial anterior: {editorialAnterior ?? "N/A"}; editorial nueva: {recurso.Editorial ?? "N/A"}. " +
                $"Año anterior: {anioAnterior?.ToString() ?? "N/A"}; año nuevo: {recurso.AnioPublicacion?.ToString() ?? "N/A"}. " +
                $"Edición anterior: {edicionAnterior ?? "N/A"}; edición nueva: {recurso.Edicion ?? "N/A"}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarRecursoBibliograficoCommand command)
    {
        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Titulo))
            return ApplicationResult.Failure("El título es obligatorio.");

        return ApplicationResult.Success();
    }
}