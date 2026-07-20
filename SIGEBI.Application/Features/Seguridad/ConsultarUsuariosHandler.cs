using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Seguridad;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class ConsultarUsuariosHandler
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ConsultarUsuariosHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<UsuarioDto>>> HandleAsync(
        ConsultarUsuariosQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var usuarios = await _usuarioRepository.BuscarAsync(
            query.TextoBusqueda,
            query.Estado,
            query.RolId,
            cancellationToken);

        var resultado = usuarios
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<UsuarioDto>>.Success(resultado);
    }

    private static UsuarioDto MapearADto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Correo = usuario.Correo,
            Matricula = usuario.Matricula,
            NumeroEmpleado = usuario.NumeroEmpleado,
            Estado = (int)usuario.Estado,
            EstadoDescripcion = usuario.Estado.ToString(),
            Roles = usuario.Roles
                .Where(usuarioRol => usuarioRol.Rol is not null)
                .Select(usuarioRol => usuarioRol.Rol!.Nombre)
                .ToList()
        };
    }
}