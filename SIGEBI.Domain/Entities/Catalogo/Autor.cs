using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class Autor : AuditableEntity
{
    private readonly List<RecursoAutor> _recursos = [];

    public string Nombre { get; private set; } = string.Empty;

    public IReadOnlyCollection<RecursoAutor> Recursos => _recursos;

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