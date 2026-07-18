using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class QuitarCategoriaRecursoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuitarCategoriaRecursoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        QuitarCategoriaRecursoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (command.CategoriaId <= 0)
            return ApplicationResult.Failure("La categoría es obligatoria.");

        var recurso = await _recursoBibliograficoRepository.ObtenerParaActualizarRelacionesAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        var recursoCategoria = recurso.Categorias.FirstOrDefault(categoria =>
            categoria.CategoriaId == command.CategoriaId);

        if (recursoCategoria is null)
            return ApplicationResult.Failure("La categoría no está asociada a este recurso bibliográfico.");

        recurso.QuitarCategoria(recursoCategoria);

        _recursoBibliograficoRepository.RemoverCategoria(recursoCategoria);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}