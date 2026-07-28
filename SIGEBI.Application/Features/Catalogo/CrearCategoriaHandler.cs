using SIGEBI.Application.Common;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearCategoriaHandler
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CrearCategoriaHandler(
        ICategoriaRepository categoriaRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearCategoriaCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult<int>.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult<int>.Failure("El nombre de la categoría es obligatorio.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult<int>.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El usuario responsable no se encuentra activo.");

        var categoriaExistente = await _categoriaRepository.ObtenerPorNombreAsync(
            command.Nombre,
            cancellationToken);

        if (categoriaExistente is not null)
            return ApplicationResult<int>.Failure("Ya existe una categoría con el mismo nombre.");

        var categoria = new Categoria(
            command.Nombre,
            command.Descripcion);

        await _categoriaRepository.AgregarAsync(
            categoria,
            cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Crear categor\u00EDa",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "Categoria",
            entidadAfectadaId: null,
            detalle:
                $"Se cre\u00F3 la categor\u00EDa {categoria.Nombre}. " +
                $"Descripci\u00F3n: {categoria.Descripcion ?? "sin descripci\u00F3n"}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(categoria.Id);
    }
}
