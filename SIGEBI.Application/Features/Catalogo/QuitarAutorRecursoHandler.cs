using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class QuitarAutorRecursoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuitarAutorRecursoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        QuitarAutorRecursoCommand command,
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

        var recursoAutor = recurso.Autores.FirstOrDefault(autor =>
            autor.AutorId == command.AutorId);

        if (recursoAutor is null)
            return ApplicationResult.Failure("El autor no está asociado a este recurso bibliográfico.");

        recurso.QuitarAutor(recursoAutor);

        _recursoBibliograficoRepository.RemoverAutor(recursoAutor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}