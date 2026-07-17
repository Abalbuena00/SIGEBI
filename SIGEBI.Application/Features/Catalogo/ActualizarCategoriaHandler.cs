using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarCategoriaHandler
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarCategoriaHandler(
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarCategoriaCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.CategoriaId <= 0)
            return ApplicationResult.Failure("La categoría es obligatoria.");

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult.Failure("El nombre de la categoría es obligatorio.");

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(
            command.CategoriaId,
            cancellationToken);

        if (categoria is null)
            return ApplicationResult.Failure("La categoría no fue encontrada.");

        var categoriaExistente = await _categoriaRepository.ObtenerPorNombreAsync(
            command.Nombre,
            cancellationToken);

        if (categoriaExistente is not null && categoriaExistente.Id != categoria.Id)
            return ApplicationResult.Failure("Ya existe otra categoría con el mismo nombre.");

        categoria.ActualizarDatos(
            command.Nombre,
            command.Descripcion);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}