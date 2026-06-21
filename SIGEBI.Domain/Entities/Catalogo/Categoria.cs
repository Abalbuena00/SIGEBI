using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class Categoria : AuditableEntity
{
    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public ICollection<RecursoCategoria> Recursos { get; private set; } = new List<RecursoCategoria>();

    private Categoria()
    {
    }

    public Categoria(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
    }

    public void ActualizarDatos(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        FechaModificacion = DateTime.UtcNow;
    }
}