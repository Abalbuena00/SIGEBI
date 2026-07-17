using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarRecursoBibliograficoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarRecursoBibliograficoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var recurso = await _recursoBibliograficoRepository.ObtenerPorIdAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        if (!string.IsNullOrWhiteSpace(command.Isbn))
        {
            var recursoExistentePorIsbn = await _recursoBibliograficoRepository.ObtenerPorIsbnAsync(
                command.Isbn,
                cancellationToken);

            if (recursoExistentePorIsbn is not null &&
                recursoExistentePorIsbn.Id != recurso.Id)
            {
                return ApplicationResult.Failure("Ya existe otro recurso bibliográfico con el mismo ISBN.");
            }
        }

        recurso.ActualizarDatos(
            command.Titulo,
            command.Isbn,
            command.Editorial,
            command.AnioPublicacion,
            command.Edicion);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarRecursoBibliograficoCommand command)
    {
        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Titulo))
            return ApplicationResult.Failure("El título es obligatorio.");

        return ApplicationResult.Success();
    }
}