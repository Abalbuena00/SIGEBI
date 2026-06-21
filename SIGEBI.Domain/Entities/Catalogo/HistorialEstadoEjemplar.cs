using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class HistorialEstadoEjemplar : BaseEntity
{
    public int EjemplarId { get; private set; }

    public EstadoEjemplar? EstadoAnterior { get; private set; }

    public EstadoEjemplar EstadoNuevo { get; private set; }

    public DateTime FechaCambio { get; private set; }

    public int? UsuarioResponsableId { get; private set; }

    public string? Motivo { get; private set; }

    public Ejemplar? Ejemplar { get; private set; }

    private HistorialEstadoEjemplar()
    {
    }

    public HistorialEstadoEjemplar(
        int ejemplarId,
        EstadoEjemplar? estadoAnterior,
        EstadoEjemplar estadoNuevo,
        int? usuarioResponsableId,
        string? motivo = null)
    {
        if (ejemplarId <= 0)
            throw new ArgumentException("El ejemplar es obligatorio.");

        EjemplarId = ejemplarId;
        EstadoAnterior = estadoAnterior;
        EstadoNuevo = estadoNuevo;
        UsuarioResponsableId = usuarioResponsableId;
        Motivo = motivo?.Trim();
        FechaCambio = DateTime.UtcNow;
    }
}