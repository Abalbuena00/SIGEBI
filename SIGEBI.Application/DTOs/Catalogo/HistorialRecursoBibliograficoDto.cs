using SIGEBI.Application.DTOs.Incidencias;
using SIGEBI.Application.DTOs.Prestamos;

namespace SIGEBI.Application.DTOs.Catalogo;

public sealed class HistorialRecursoBibliograficoDto
{
    public int RecursoBibliograficoId { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public IReadOnlyList<HistorialEstadoEjemplarDto> HistorialEstados { get; set; } = [];
    public IReadOnlyList<PrestamoDto> Prestamos { get; set; } = [];
    public IReadOnlyList<IncidenciaEjemplarDto> Incidencias { get; set; } = [];
}