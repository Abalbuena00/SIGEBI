using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Configuracion;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Configuracion;

public sealed class ConsultarParametrosSistemaHandler
{
    private readonly IParametroSistemaRepository _parametroSistemaRepository;

    public ConsultarParametrosSistemaHandler(
        IParametroSistemaRepository parametroSistemaRepository)
    {
        _parametroSistemaRepository = parametroSistemaRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<ParametroSistemaDto>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var parametros = await _parametroSistemaRepository.ObtenerTodosAsync(
            cancellationToken);

        var resultado = parametros
            .Where(parametro => parametro.Activo)
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<ParametroSistemaDto>>.Success(resultado);
    }

    private static ParametroSistemaDto MapearADto(ParametroSistema parametro)
    {
        return new ParametroSistemaDto
        {
            Id = parametro.Id,
            Clave = parametro.Clave,
            Valor = parametro.Valor,
            Descripcion = parametro.Descripcion,
            Activo = parametro.Activo
        };
    }
}
