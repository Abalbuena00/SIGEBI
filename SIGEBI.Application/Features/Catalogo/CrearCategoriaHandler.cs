using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearCategoriaHandler
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearCategoriaHandler(
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearCategoriaCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult<int>.Failure("El nombre de la categoría es obligatorio.");

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(categoria.Id);
    }
}