namespace SIGEBI.Application.Features.Incidencias;

public sealed class CerrarIncidenciaEjemplarCommand
{
    public int IncidenciaId { get; init; }

    public int UsuarioCierreId { get; init; }
}
