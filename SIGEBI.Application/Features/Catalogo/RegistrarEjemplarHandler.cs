using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class RegistrarEjemplarHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarEjemplarHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IEjemplarRepository ejemplarRepository,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _ejemplarRepository = ejemplarRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        RegistrarEjemplarCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return ApplicationResult<int>.Failure(validacion.Error!);

        var recurso = await _recursoBibliograficoRepository.ObtenerPorIdAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult<int>.Failure("El recurso bibliográfico no fue encontrado.");

        var ejemplarExistente = await _ejemplarRepository.ObtenerPorCodigoInternoAsync(
            command.CodigoInterno,
            cancellationToken);

        if (ejemplarExistente is not null)
            return ApplicationResult<int>.Failure("Ya existe un ejemplar con el mismo código interno.");

        var ejemplar = new Ejemplar(
            command.RecursoBibliograficoId,
            command.CodigoInterno,
            command.EstadoFisico);

        await _ejemplarRepository.AgregarAsync(
            ejemplar,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(ejemplar.Id);
    }

    private static ApplicationResult ValidarCommand(
        RegistrarEjemplarCommand command)
    {
        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.CodigoInterno))
            return ApplicationResult.Failure("El código interno del ejemplar es obligatorio.");

        return ApplicationResult.Success();
    }
}