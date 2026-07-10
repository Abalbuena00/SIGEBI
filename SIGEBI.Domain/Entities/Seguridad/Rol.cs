using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Seguridad;

public sealed class Rol : AuditableEntity
{
    private readonly List<UsuarioRol> _usuarios = [];

    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public IReadOnlyCollection<UsuarioRol> Usuarios => _usuarios;

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