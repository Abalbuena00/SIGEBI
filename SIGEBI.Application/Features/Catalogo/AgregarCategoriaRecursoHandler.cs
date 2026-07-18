using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class AgregarCategoriaRecursoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarCategoriaRecursoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        AgregarCategoriaRecursoCommand command,
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

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(
            command.CategoriaId,
            cancellationToken);

        if (categoria is null)
            return ApplicationResult.Failure("La categoría no fue encontrada.");

        var recursoCategoria = new RecursoCategoria(
            recurso.Id,
            categoria.Id);

        recurso.AgregarCategoria(recursoCategoria);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}