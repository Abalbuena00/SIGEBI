using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Configuracion;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Configuracion;

public sealed class ConsultarPoliticasPrestamoHandler
{
    private readonly IPoliticaPrestamoRepository _politicaPrestamoRepository;

    public ConsultarPoliticasPrestamoHandler(
        IPoliticaPrestamoRepository politicaPrestamoRepository)
    {
        _politicaPrestamoRepository = politicaPrestamoRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<PoliticaPrestamoDto>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var politicas = await _politicaPrestamoRepository.ObtenerTodosAsync(
            cancellationToken);

        var resultado = politicas
            .Where(politica => politica.Activo)
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<PoliticaPrestamoDto>>.Success(resultado);
    }

    private static PoliticaPrestamoDto MapearADto(PoliticaPrestamo politica)
    {
        return new PoliticaPrestamoDto
        {
            Id = politica.Id,
            TipoMiembro = (int)politica.TipoMiembro,
            TipoMiembroDescripcion = politica.TipoMiembro.ToString(),
            MaximoPrestamosActivos = politica.MaximoPrestamosActivos,
            DiasDuracionPrestamo = politica.DiasDuracionPrestamo,
            HorasReservaTemporal = politica.HorasReservaTemporal,
            DiasSuspensionPorDiaRetraso = politica.DiasSuspensionPorDiaRetraso,
            PermitePrestamoConVencidos = politica.PermitePrestamoConVencidos,
            PenalizaRetraso = politica.PenalizaRetraso,
            Activo = politica.Activo
        };
    }
}
