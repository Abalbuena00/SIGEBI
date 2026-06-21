namespace SIGEBI.Domain.Base;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime FechaCreacion { get; protected set; } = DateTime.UtcNow;

    public DateTime? FechaModificacion { get; protected set; }

    public bool Activo { get; protected set; } = true;

    public void Desactivar()
    {
        Activo = false;
        FechaModificacion = DateTime.UtcNow;
    }
}