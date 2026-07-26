using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class InactivarRecursoBibliograficoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public InactivarRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoRepository,
        IPrestamoRepository prestamoRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _recursoRepository = recursoRepository;
        _prestamoRepository = prestamoRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> EjecutarInactivacionAsync(
        InactivarRecursoBibliograficoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var validacion = ValidarCommand(command);
        if (!validacion.IsSuccess)
            return validacion;

        return await EjecutarInactivacionAsync(command, cancellationToken);
    }

    private async Task<ApplicationResult> ValidarEntidadesAsync(
        InactivarRecursoBibliograficoCommand command,
        CancellationToken cancellationToken)
    {
        return await ValidarUsuarioAsync(command, cancellationToken);
    }

    private async Task<ApplicationResult> ValidarUsuarioAsync(
        InactivarRecursoBibliograficoCommand command,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId, cancellationToken);
        if (usuario is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");
        if (usuario.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var recurso = await _recursoRepository.ObtenerPorIdAsync(
            command.RecursoBibliograficoId, cancellationToken);
        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");
        if (!recurso.Activo)
            return ApplicationResult.Failure("El recurso bibliográfico ya se encuentra inactivo.");

        bool tienePrestamos = await _prestamoRepository
            .ExistePrestamoAbiertoPorRecursoAsync(recurso.Id, cancellationToken);
        if (tienePrestamos)
            return ApplicationResult.Failure(
                "No se puede inactivar el recurso porque tiene préstamos activos o vencidos pendientes.");

        string? motivo = string.IsNullOrWhiteSpace(command.Motivo)
            ? null
            : command.Motivo.Trim();

        recurso.Desactivar();

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Catálogo",
            accion: "Inactivar recurso bibliográfico",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: recurso.Id,
            detalle: $"Se inactivó el recurso '{recurso.Titulo}' con código interno " +
                $"{recurso.CodigoInterno}. Motivo: {motivo ?? "no informado"}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        InactivarRecursoBibliograficoCommand command)
    {
        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");
        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        return ApplicationResult.Success();
    }
}
