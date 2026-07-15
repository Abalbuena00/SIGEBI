using SIGEBI.Application.Common;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class MarcarPrestamosVencidosHandler
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarPrestamosVencidosHandler(
        IPrestamoRepository prestamoRepository,
        IUnitOfWork unitOfWork)
    {
        _prestamoRepository = prestamoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        MarcarPrestamosVencidosCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        DateTime fechaReferencia = command.FechaReferencia ?? DateTime.UtcNow;

        var prestamosVencidos = await _prestamoRepository.ObtenerVencidosAsync(
            fechaReferencia,
            cancellationToken);

        int cantidadMarcados = 0;

        foreach (var prestamo in prestamosVencidos)
        {
            var resultado = prestamo.MarcarComoVencido();

            if (!resultado.IsSuccess)
                continue;

            cantidadMarcados++;
        }

        if (cantidadMarcados > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(cantidadMarcados);
    }
}