using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class RecursoBibliografico : AuditableEntity
{
    public string CodigoInterno { get; private set; } = string.Empty;

    public string Titulo { get; private set; } = string.Empty;

    public string? Isbn { get; private set; }

    public string? Editorial { get; private set; }

    public int? AnioPublicacion { get; private set; }

    public string? Edicion { get; private set; }

    public ICollection<Ejemplar> Ejemplares { get; private set; } = new List<Ejemplar>();

    public ICollection<RecursoAutor> Autores { get; private set; } = new List<RecursoAutor>();

    public ICollection<RecursoCategoria> Categorias { get; private set; } = new List<RecursoCategoria>();

    private RecursoBibliografico()
    {
    }

    public RecursoBibliografico(
        string codigoInterno,
        string titulo,
        string? isbn = null,
        string? editorial = null,
        int? anioPublicacion = null,
        string? edicion = null)
    {
        if (string.IsNullOrWhiteSpace(codigoInterno))
            throw new ArgumentException("El código interno es obligatorio.");

        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título es obligatorio.");

        CodigoInterno = codigoInterno.Trim();
        Titulo = titulo.Trim();
        Isbn = isbn?.Trim();
        Editorial = editorial?.Trim();
        AnioPublicacion = anioPublicacion;
        Edicion = edicion?.Trim();
    }

    public void ActualizarDatos(
        string titulo,
        string? isbn,
        string? editorial,
        int? anioPublicacion,
        string? edicion)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título es obligatorio.");

        Titulo = titulo.Trim();
        Isbn = isbn?.Trim();
        Editorial = editorial?.Trim();
        AnioPublicacion = anioPublicacion;
        Edicion = edicion?.Trim();
        FechaModificacion = DateTime.UtcNow;
    }
}