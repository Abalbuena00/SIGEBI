using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Services.Auditoria;

public sealed class AuditoriaService : IAuditoriaService
{
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository;

    public AuditoriaService(IRegistroAuditoriaRepository registroAuditoriaRepository)
    {
        _registroAuditoriaRepository = registroAuditoriaRepository;
    }

    public async Task RegistrarAsync(
        int? usuarioId,
        string modulo,
        string accion,
        ResultadoAuditoria resultado,
        string? entidadAfectada = null,
        int? entidadAfectadaId = null,
        string? detalle = null,
        string? direccionIp = null,
        string? origen = null,
        CancellationToken cancellationToken = default)
    {
        var registro = new RegistroAuditoria(
            usuarioId,
            modulo,
            accion,
            resultado,
            entidadAfectada,
            entidadAfectadaId,
            detalle,
            direccionIp,
            origen);

        await _registroAuditoriaRepository.AgregarAsync(
            registro,
            cancellationToken);
    }
}