using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Configuracion;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Configuracion;

public sealed class ConsultarParametroSistemaPorClaveHandler
{
    private readonly IParametroSistemaRepository _parametroSistemaRepository;

    public ConsultarParametroSistemaPorClaveHandler(
        IParametroSistemaRepository parametroSistemaRepository)
    {
        _parametroSistemaRepository = parametroSistemaRepository;
    }

    public async Task<ApplicationResult<ParametroSistemaDto>> HandleAsync(
        ConsultarParametroSistemaPorClaveQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.IsNullOrWhiteSpace(query.Clave))
        {
            return ApplicationResult<ParametroSistemaDto>.Failure(
                "La clave del parámetro es obligatoria.");
        }

        var parametro = await _parametroSistemaRepository.ObtenerPorClaveAsync(
            query.Clave,
            cancellationToken);

        if (parametro is null)
        {
            return ApplicationResult<ParametroSistemaDto>.Failure(
                "El parámetro del sistema no fue encontrado.");
        }

        return ApplicationResult<ParametroSistemaDto>.Success(MapearADto(parametro));
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
