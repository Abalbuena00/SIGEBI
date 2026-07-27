using SIGEBI.Application.DTOs.Incidencias;
using SIGEBI.Application.DTOs.Penalizaciones;
using SIGEBI.Application.DTOs.Prestamos;

namespace SIGEBI.Application.DTOs.Seguridad;

public sealed class HistorialUsuarioDto
{
    public int UsuarioId { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string? Matricula { get; init; }
    public string? NumeroEmpleado { get; init; }
    public int Estado { get; init; }
    public string EstadoDescripcion { get; init; } = string.Empty;
    public IReadOnlyList<SolicitudPrestamoDto> Solicitudes { get; init; } = [];
    public IReadOnlyList<PrestamoDto> Prestamos { get; init; } = [];
    public IReadOnlyList<PenalizacionDto> Penalizaciones { get; init; } = [];
    public IReadOnlyList<IncidenciaEjemplarDto> Incidencias { get; init; } = [];
}
