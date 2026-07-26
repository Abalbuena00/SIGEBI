namespace SIGEBI.Application.Features.Incidencias;

public sealed class ConsultarIncidenciasEjemplarQuery
{
    public int EjemplarId { get; init; }

    public bool? Cerrada { get; init; }
}
