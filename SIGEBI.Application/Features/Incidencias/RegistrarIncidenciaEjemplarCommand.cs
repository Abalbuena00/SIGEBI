using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Incidencias;

public sealed class RegistrarIncidenciaEjemplarCommand
{
    public int EjemplarId { get; init; }

    public int UsuarioReportaId { get; init; }

    public TipoIncidenciaEjemplar Tipo { get; init; }

    public string Descripcion { get; init; } = string.Empty;

    public int? PrestamoId { get; init; }
}
