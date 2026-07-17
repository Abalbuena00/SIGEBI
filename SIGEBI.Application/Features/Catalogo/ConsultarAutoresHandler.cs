using SIGEBI.Application.Common;
using SIGEBI.Application.DTOs.Catalogo;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarAutoresHandler
{
    private readonly IAutorRepository _autorRepository;

    public ConsultarAutoresHandler(IAutorRepository autorRepository)
    {
        _autorRepository = autorRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<AutorDto>>> HandleAsync(
        ConsultarAutoresQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var autores = await _autorRepository.BuscarAsync(
            query.Nombre,
            cancellationToken);

        var resultado = autores
            .Select(MapearADto)
            .ToList();

        return ApplicationResult<IReadOnlyList<AutorDto>>.Success(resultado);
    }

    private static AutorDto MapearADto(Autor autor)
    {
        return new AutorDto
        {
            Id = autor.Id,
            Nombre = autor.Nombre
        };
    }
}