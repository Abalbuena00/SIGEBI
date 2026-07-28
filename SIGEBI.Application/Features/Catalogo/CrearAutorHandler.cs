using SIGEBI.Application.Common;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearAutorHandler
{
    private readonly IAutorRepository _autorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CrearAutorHandler(
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

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearAutorCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult<int>.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult<int>.Failure("El nombre del autor es obligatorio.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult<int>.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El usuario responsable no se encuentra activo.");

        var autorExistente = await _autorRepository.ObtenerPorNombreAsync(
            command.Nombre,
            cancellationToken);

        if (autorExistente is not null)
            return ApplicationResult<int>.Failure("Ya existe un autor con el mismo nombre.");

        var autor = new Autor(command.Nombre);

        await _autorRepository.AgregarAsync(
            autor,
            cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Crear autor",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Autor",
            entidadAfectadaId: null,
            detalle: $"Se cre\u00F3 el autor {autor.Nombre}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(autor.Id);
    }
}
