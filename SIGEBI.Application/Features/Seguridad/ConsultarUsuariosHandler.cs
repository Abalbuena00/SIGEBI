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

    public async Task<ApplicationResult<PagedResult<UsuarioDto>>> HandleAsync(
    ConsultarUsuariosQuery query,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var validacion = ValidarPaginacion(
            query.PageNumber,
            query.PageSize);

        if (!validacion.IsSuccess)
            return ApplicationResult<PagedResult<UsuarioDto>>.Failure(validacion.Error!);

        var resultadoConsulta = await _usuarioRepository.BuscarAsync(
            query.TextoBusqueda,
            query.Estado,
            query.RolId,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var items = resultadoConsulta.Items
            .Select(MapearADto)
            .ToList();

        var resultado = new PagedResult<UsuarioDto>(
            items,
            resultadoConsulta.TotalCount,
            query.PageNumber,
            query.PageSize);

        return ApplicationResult<PagedResult<UsuarioDto>>.Success(resultado);
    }

    private static ApplicationResult ValidarPaginacion(
    int pageNumber,
    int pageSize)
    {
        if (pageNumber <= 0)
            return ApplicationResult.Failure("El número de página debe ser mayor que cero.");

        if (pageSize <= 0)
            return ApplicationResult.Failure("El tamaño de página debe ser mayor que cero.");

        if (pageSize > 100)
            return ApplicationResult.Failure("El tamaño de página no puede ser mayor a 100 registros.");

        return ApplicationResult.Success();
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