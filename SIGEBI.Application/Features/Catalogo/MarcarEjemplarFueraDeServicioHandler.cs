using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class MarcarEjemplarFueraDeServicioHandler
{
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarEjemplarFueraDeServicioHandler(
        IEjemplarRepository ejemplarRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _ejemplarRepository = ejemplarRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        MarcarEjemplarFueraDeServicioCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var ejemplar = await _ejemplarRepository.ObtenerPorIdAsync(
            command.EjemplarId,
            cancellationToken);

        if (ejemplar is null)
            return ApplicationResult.Failure("El ejemplar no fue encontrado.");

        if (ejemplar.Estado == EstadoEjemplar.Prestado)
            return ApplicationResult.Failure("No se puede marcar fuera de servicio un ejemplar prestado.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        EstadoEjemplar estadoAnterior = ejemplar.Estado;

        ejemplar.MarcarFueraDeServicio(command.Motivo);

        var historial = new HistorialEstadoEjemplar(
            ejemplar.Id,
            estadoAnterior,
            ejemplar.Estado,
            command.UsuarioResponsableId,
            command.Motivo);

        ejemplar.RegistrarHistorialEstado(historial);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        MarcarEjemplarFueraDeServicioCommand command)
    {
        if (command.EjemplarId <= 0)
            return ApplicationResult.Failure("El ejemplar es obligatorio.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Motivo))
            return ApplicationResult.Failure("Debe indicar el motivo para marcar el ejemplar fuera de servicio.");

        return ApplicationResult.Success();
    }
}