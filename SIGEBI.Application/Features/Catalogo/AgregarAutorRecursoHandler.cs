using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class AgregarAutorRecursoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarAutorRecursoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IAutorRepository autorRepository,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _autorRepository = autorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        AgregarAutorRecursoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (command.AutorId <= 0)
            return ApplicationResult.Failure("El autor es obligatorio.");

        var recurso = await _recursoBibliograficoRepository.ObtenerParaActualizarRelacionesAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        var autor = await _autorRepository.ObtenerPorIdAsync(
            command.AutorId,
            cancellationToken);

        if (autor is null)
            return ApplicationResult.Failure("El autor no fue encontrado.");

        var recursoAutor = new RecursoAutor(
            recurso.Id,
            autor.Id);

        recurso.AgregarAutor(recursoAutor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}