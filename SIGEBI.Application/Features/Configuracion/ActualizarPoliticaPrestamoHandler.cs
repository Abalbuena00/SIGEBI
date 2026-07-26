using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Configuracion;

public sealed class ActualizarPoliticaPrestamoHandler
{
    private readonly IPoliticaPrestamoRepository _politicaPrestamoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarPoliticaPrestamoHandler(
        IPoliticaPrestamoRepository politicaPrestamoRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _politicaPrestamoRepository = politicaPrestamoRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        ActualizarPoliticaPrestamoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return validacion;

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var politica = await _politicaPrestamoRepository.ObtenerPorIdAsync(
            command.PoliticaPrestamoId,
            cancellationToken);

        if (politica is null)
            return ApplicationResult.Failure("La política de préstamo no fue encontrada.");

        string valoresAnteriores =
            $"Máximo préstamos: {politica.MaximoPrestamosActivos}; " +
            $"duración: {politica.DiasDuracionPrestamo} días; " +
            $"reserva: {politica.HorasReservaTemporal} horas; " +
            $"suspensión por retraso: {politica.DiasSuspensionPorDiaRetraso} días; " +
            $"permite vencidos: {politica.PermitePrestamoConVencidos}; " +
            $"penaliza retraso: {politica.PenalizaRetraso}";

        politica.Actualizar(
            command.MaximoPrestamosActivos,
            command.DiasDuracionPrestamo,
            command.HorasReservaTemporal,
            command.DiasSuspensionPorDiaRetraso,
            command.PermitePrestamoConVencidos,
            command.PenalizaRetraso);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Configuración",
            accion: "Actualizar política de préstamo",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "PoliticaPrestamo",
            entidadAfectadaId: politica.Id,
            detalle:
                $"Se actualizó la política para {politica.TipoMiembro}. " +
                $"Valores anteriores: {valoresAnteriores}. " +
                $"Valores nuevos: máximo préstamos: {politica.MaximoPrestamosActivos}; " +
                $"duración: {politica.DiasDuracionPrestamo} días; " +
                $"reserva: {politica.HorasReservaTemporal} horas; " +
                $"suspensión por retraso: {politica.DiasSuspensionPorDiaRetraso} días; " +
                $"permite vencidos: {politica.PermitePrestamoConVencidos}; " +
                $"penaliza retraso: {politica.PenalizaRetraso}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private static ApplicationResult ValidarCommand(
        ActualizarPoliticaPrestamoCommand command)
    {
        if (command.PoliticaPrestamoId <= 0)
            return ApplicationResult.Failure("La política de préstamo es obligatoria.");

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (command.MaximoPrestamosActivos <= 0)
        {
            return ApplicationResult.Failure(
                "El máximo de préstamos activos debe ser mayor que cero.");
        }

        if (command.DiasDuracionPrestamo <= 0)
        {
            return ApplicationResult.Failure(
                "La duración del préstamo debe ser mayor que cero.");
        }

        if (command.HorasReservaTemporal <= 0)
        {
            return ApplicationResult.Failure(
                "La reserva temporal debe ser mayor que cero.");
        }

        if (command.DiasSuspensionPorDiaRetraso < 0)
        {
            return ApplicationResult.Failure(
                "Los días de suspensión no pueden ser negativos.");
        }

        return ApplicationResult.Success();
    }
}
