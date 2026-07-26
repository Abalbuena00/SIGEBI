using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Auditoria;

public sealed class ConsultarAuditoriaQuery
{
    public int? UsuarioId { get; init; }
    public string? Modulo { get; init; }
    public ResultadoAuditoria? Resultado { get; init; }
    public string? EntidadAfectada { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
