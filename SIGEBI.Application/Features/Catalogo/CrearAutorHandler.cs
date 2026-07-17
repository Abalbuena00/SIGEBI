using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearAutorHandler
{
    private readonly IAutorRepository _autorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearAutorHandler(
        IAutorRepository autorRepository,
        IUnitOfWork unitOfWork)
    {
        _autorRepository = autorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearAutorCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Nombre))
            return ApplicationResult<int>.Failure("El nombre del autor es obligatorio.");

        var autorExistente = await _autorRepository.ObtenerPorNombreAsync(
            command.Nombre,
            cancellationToken);

        if (autorExistente is not null)
            return ApplicationResult<int>.Failure("Ya existe un autor con el mismo nombre.");

        var autor = new Autor(command.Nombre);

        await _autorRepository.AgregarAsync(
            autor,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(autor.Id);
    }
}