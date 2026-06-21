using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class Autor : AuditableEntity
{
    public string Nombre { get; private set; } = string.Empty;

    public ICollection<RecursoAutor> Recursos { get; private set; } = new List<RecursoAutor>();

    private Autor()
    {
    }

    public Autor(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del autor es obligatorio.");

        Nombre = nombre.Trim();
    }

    public void ActualizarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del autor es obligatorio.");

        Nombre = nombre.Trim();
        FechaModificacion = DateTime.UtcNow;
    }
}