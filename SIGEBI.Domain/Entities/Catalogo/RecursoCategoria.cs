using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class RecursoCategoria : BaseEntity
{
    public int RecursoBibliograficoId { get; private set; }

    public int CategoriaId { get; private set; }

    public RecursoBibliografico? RecursoBibliografico { get; private set; }

    public Categoria? Categoria { get; private set; }

    private RecursoCategoria()
    {
    }

    public RecursoCategoria(int recursoBibliograficoId, int categoriaId)
    {
        if (recursoBibliograficoId <= 0)
            throw new ArgumentException("El recurso bibliográfico es obligatorio.");

        if (categoriaId <= 0)
            throw new ArgumentException("La categoría es obligatoria.");

        RecursoBibliograficoId = recursoBibliograficoId;
        CategoriaId = categoriaId;
    }
}