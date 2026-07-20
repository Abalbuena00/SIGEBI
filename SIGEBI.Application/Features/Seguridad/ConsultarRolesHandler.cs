using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Seguridad;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class ConsultarRolesHandler
{
    private readonly IRolRepository _rolRepository;

    public ConsultarRolesHandler(IRolRepository rolRepository)
    {
        _rolRepository = rolRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<RolDto>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {

        var roles = await _rolRepository.ObtenerTodosAsync(cancellationToken);

        var resultado = roles
            .Where(rol => rol.Activo)
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<RolDto>>.Success(resultado);
    }

    private static RolDto MapearADto(Rol rol)
    {
        return new RolDto
        {
            Id = rol.Id,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion
        };
    }
}