using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Prestamos;

public sealed class ConsultarHistorialPrestamosQuery
{
    public int? UsuarioId { get; init; }
    public int? RecursoBibliograficoId { get; init; }
    public int? EjemplarId { get; init; }
    public EstadoPrestamo? Estado { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
