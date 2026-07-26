using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Auditoria;
using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Auditoria;

public sealed class ConsultarAuditoriaHandler
{
    private readonly IRegistroAuditoriaRepository _repository;

    public ConsultarAuditoriaHandler(IRegistroAuditoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApplicationResult<PagedResult<RegistroAuditoriaDto>>> HandleAsync(
        ConsultarAuditoriaQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var validacion = ValidarQuery(query);
        if (!validacion.IsSuccess)
            return ApplicationResult<PagedResult<RegistroAuditoriaDto>>.Failure(validacion.Error!);

        var consulta = await _repository.ConsultarAsync(
            query.UsuarioId, query.Modulo, query.Resultado, query.EntidadAfectada,
            query.FechaDesde, query.FechaHasta, query.PageNumber, query.PageSize,
            cancellationToken);

        var items = consulta.Items.Select(MapearADto).ToList();
        var resultado = new PagedResult<RegistroAuditoriaDto>(
            items, consulta.TotalItems, query.PageNumber, query.PageSize);

        return ApplicationResult<PagedResult<RegistroAuditoriaDto>>.Success(resultado);
    }

    private static RegistroAuditoriaDto MapearADto(RegistroAuditoria registro)
    {
        return new RegistroAuditoriaDto
        {
            Id = registro.Id,
            UsuarioId = registro.UsuarioId,
            Modulo = registro.Modulo,
            Accion = registro.Accion,
            EntidadAfectada = registro.EntidadAfectada,
            EntidadAfectadaId = registro.EntidadAfectadaId,
            Resultado = (int)registro.Resultado,
            ResultadoDescripcion = registro.Resultado.ToString(),
            Detalle = registro.Detalle,
            DireccionIp = registro.DireccionIp,
            Origen = registro.Origen,
            FechaRegistro = registro.FechaRegistro
        };
    }

    private static ApplicationResult ValidarQuery(ConsultarAuditoriaQuery query)
    {
        if (query.PageNumber <= 0)
            return ApplicationResult.Failure("El número de página debe ser mayor que cero.");
        if (query.PageSize <= 0)
            return ApplicationResult.Failure("El tamaño de página debe ser mayor que cero.");
        if (query.PageSize > 100)
            return ApplicationResult.Failure("El tamaño de página no puede superar 100 registros.");
        if (query.UsuarioId.HasValue && query.UsuarioId.Value <= 0)
            return ApplicationResult.Failure("El usuario indicado no es válido.");
        if (query.Resultado.HasValue &&
            !Enum.IsDefined(typeof(ResultadoAuditoria), query.Resultado.Value))
            return ApplicationResult.Failure("El resultado de auditoría indicado no es válido.");
        if (query.FechaDesde.HasValue && query.FechaHasta.HasValue &&
            query.FechaDesde.Value > query.FechaHasta.Value)
            return ApplicationResult.Failure("La fecha desde no puede ser mayor que la fecha hasta.");

        return ApplicationResult.Success();
    }
}
