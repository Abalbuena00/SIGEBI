using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Seguridad;

public sealed class UsuarioRol : BaseEntity
{
    public int UsuarioId { get; private set; }

    public int RolId { get; private set; }

    public DateTime FechaAsignacion { get; private set; }

    public Usuario? Usuario { get; private set; }

    public Rol? Rol { get; private set; }

    private UsuarioRol()
    {
    }

    public UsuarioRol(int usuarioId, int rolId)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El usuario es obligatorio.");

        if (rolId <= 0)
            throw new ArgumentException("El rol es obligatorio.");

        UsuarioId = usuarioId;
        RolId = rolId;
        FechaAsignacion = DateTime.UtcNow;
    }
}