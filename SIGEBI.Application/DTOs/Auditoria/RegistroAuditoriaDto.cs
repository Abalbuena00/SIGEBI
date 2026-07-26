namespace SIGEBI.Application.DTOs.Auditoria;

public sealed class RegistroAuditoriaDto
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string? EntidadAfectada { get; set; }
    public int? EntidadAfectadaId { get; set; }
    public int Resultado { get; set; }
    public string ResultadoDescripcion { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public string? DireccionIp { get; set; }
    public string? Origen { get; set; }
    public DateTime FechaRegistro { get; set; }
}
