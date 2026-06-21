using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Auditoria;

public sealed class RegistroAuditoria : BaseEntity
{
    public int? UsuarioId { get; private set; }

    public string Modulo { get; private set; } = string.Empty;

    public string Accion { get; private set; } = string.Empty;

    public string? EntidadAfectada { get; private set; }

    public int? EntidadAfectadaId { get; private set; }

    public ResultadoAuditoria Resultado { get; private set; }

    public string? Detalle { get; private set; }

    public string? DireccionIp { get; private set; }

    public string? Origen { get; private set; }

    public DateTime FechaRegistro { get; private set; }

    private RegistroAuditoria()
    {
    }

    public RegistroAuditoria(
        int? usuarioId,
        string modulo,
        string accion,
        ResultadoAuditoria resultado,
        string? entidadAfectada = null,
        int? entidadAfectadaId = null,
        string? detalle = null,
        string? direccionIp = null,
        string? origen = null)
    {
        if (string.IsNullOrWhiteSpace(modulo))
            throw new ArgumentException("El módulo es obligatorio.");

        if (string.IsNullOrWhiteSpace(accion))
            throw new ArgumentException("La acción es obligatoria.");

        UsuarioId = usuarioId;
        Modulo = modulo.Trim();
        Accion = accion.Trim();
        Resultado = resultado;
        EntidadAfectada = entidadAfectada?.Trim();
        EntidadAfectadaId = entidadAfectadaId;
        Detalle = detalle?.Trim();
        DireccionIp = direccionIp?.Trim();
        Origen = origen?.Trim();
        FechaRegistro = DateTime.UtcNow;
    }
}