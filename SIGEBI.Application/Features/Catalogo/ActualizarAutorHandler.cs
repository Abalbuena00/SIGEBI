using SIGEBI.Application.Common;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarAutorHandler
{
    private readonly IAutorRepository _autorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarAutorHandler(
        IAutorRepository autorRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _autorRepository = autorRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarAutorCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (command.AutorId <= 0)
            return ApplicationResult.Failure("El autor es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult.Failure("El nombre del autor es obligatorio.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var autor = await _autorRepository.ObtenerPorIdAsync(
            command.AutorId,
            cancellationToken);

        if (autor is null)
            return ApplicationResult.Failure("El autor no fue encontrado.");

        var autorExistente = await _autorRepository.ObtenerPorNombreAsync(
            command.Nombre,
            cancellationToken);

        if (autorExistente is not null && autorExistente.Id != autor.Id)
            return ApplicationResult.Failure("Ya existe otro autor con el mismo nombre.");

        string nombreAnterior = autor.Nombre;

        autor.ActualizarNombre(command.Nombre);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Actualizar autor",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Autor",
            entidadAfectadaId: autor.Id,
            detalle: $"Nombre anterior: {nombreAnterior}; nombre nuevo: {autor.Nombre}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}
