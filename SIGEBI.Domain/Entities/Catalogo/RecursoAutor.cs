using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class RecursoAutor : BaseEntity
{
    public int RecursoBibliograficoId { get; private set; }

    public int AutorId { get; private set; }

    public RecursoBibliografico? RecursoBibliografico { get; private set; }

    public Autor? Autor { get; private set; }

    private RecursoAutor()
    {
    }

    public RecursoAutor(int recursoBibliograficoId, int autorId)
    {
        if (recursoBibliograficoId <= 0)
            throw new ArgumentException("El recurso bibliográfico es obligatorio.");

        if (autorId <= 0)
            throw new ArgumentException("El autor es obligatorio.");

        RecursoBibliograficoId = recursoBibliograficoId;
        AutorId = autorId;
    }

    public RecursoAutor(RecursoBibliografico recursoBibliografico, Autor autor)
    {
        ArgumentNullException.ThrowIfNull(recursoBibliografico);
        ArgumentNullException.ThrowIfNull(autor);

        if (autor.Id <= 0)
            throw new ArgumentException("El autor debe existir antes de asociarlo.");

        RecursoBibliografico = recursoBibliografico;
        RecursoBibliograficoId = recursoBibliografico.Id;
        Autor = autor;
        AutorId = autor.Id;
    }
}
