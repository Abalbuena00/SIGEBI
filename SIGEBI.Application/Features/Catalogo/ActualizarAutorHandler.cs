using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarAutorHandler
{
    private readonly IAutorRepository _autorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarAutorHandler(
        IAutorRepository autorRepository,
        IUnitOfWork unitOfWork)
    {
        _autorRepository = autorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarAutorCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.AutorId <= 0)
            return ApplicationResult.Failure("El autor es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult.Failure("El nombre del autor es obligatorio.");

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

        autor.ActualizarNombre(command.Nombre);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}