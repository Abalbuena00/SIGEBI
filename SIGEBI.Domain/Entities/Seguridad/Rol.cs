using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Seguridad;

public sealed class Rol : AuditableEntity
{
    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public ICollection<UsuarioRol> Usuarios { get; private set; } = new List<UsuarioRol>();

    private Rol()
    {
    }

    public Rol(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del rol es obligatorio.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
    }

    public void ActualizarDescripcion(string? descripcion)
    {
        Descripcion = descripcion?.Trim();
        FechaModificacion = DateTime.UtcNow;
    }
}