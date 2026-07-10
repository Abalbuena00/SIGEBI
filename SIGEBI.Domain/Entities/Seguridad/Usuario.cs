using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Seguridad;

public sealed class Usuario : AuditableEntity
{
    private readonly List<UsuarioRol> _roles = [];

    public string NombreCompleto { get; private set; } = string.Empty;

    public string Correo { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string? Matricula { get; private set; }

    public string? NumeroEmpleado { get; private set; }

    public EstadoUsuario Estado { get; private set; }

    public IReadOnlyCollection<UsuarioRol> Roles => _roles;

    private Usuario()
    {
    }

    public Usuario(
        string nombreCompleto,
        string correo,
        string passwordHash,
        string? matricula = null,
        string? numeroEmpleado = null)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(correo))
            throw new ArgumentException("El correo es obligatorio.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("La contraseña protegida es obligatoria.");

        NombreCompleto = nombreCompleto.Trim();
        Correo = correo.Trim().ToLower();
        PasswordHash = passwordHash;
        Matricula = matricula?.Trim();
        NumeroEmpleado = numeroEmpleado?.Trim();
        Estado = EstadoUsuario.Activo;
    }

    public void CambiarEstado(EstadoUsuario nuevoEstado)
    {
        Estado = nuevoEstado;
        FechaModificacion = DateTime.UtcNow;
    }

    public void ActualizarDatos(
        string nombreCompleto,
        string? matricula,
        string? numeroEmpleado)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre completo es obligatorio.");

        NombreCompleto = nombreCompleto.Trim();
        Matricula = matricula?.Trim();
        NumeroEmpleado = numeroEmpleado?.Trim();
        FechaModificacion = DateTime.UtcNow;
    }

    public void AgregarRol(UsuarioRol usuarioRol)
    {
        ArgumentNullException.ThrowIfNull(usuarioRol);

        bool existeRol = _roles.Any(registro =>
            registro.RolId == usuarioRol.RolId);

        if (existeRol)
            throw new InvalidOperationException("El usuario ya tiene asignado este rol.");

        _roles.Add(usuarioRol);
        FechaModificacion = DateTime.UtcNow;
    }

    public void QuitarRol(UsuarioRol usuarioRol)
    {
        ArgumentNullException.ThrowIfNull(usuarioRol);

        bool removido = _roles.Remove(usuarioRol);

        if (removido)
            FechaModificacion = DateTime.UtcNow;
    }
}