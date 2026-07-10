using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Catalogo;

public sealed class RecursoBibliografico : AuditableEntity
{
    private readonly List<Ejemplar> _ejemplares = [];
    private readonly List<RecursoAutor> _autores = [];
    private readonly List<RecursoCategoria> _categorias = [];

    public string CodigoInterno { get; private set; } = string.Empty;

    public string Titulo { get; private set; } = string.Empty;

    public string? Isbn { get; private set; }

    public string? Editorial { get; private set; }

    public int? AnioPublicacion { get; private set; }

    public string? Edicion { get; private set; }

    public IReadOnlyCollection<Ejemplar> Ejemplares => _ejemplares;

    public IReadOnlyCollection<RecursoAutor> Autores => _autores;

    public IReadOnlyCollection<RecursoCategoria> Categorias => _categorias;

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

        ValidarAnioPublicacion(anioPublicacion);

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

        ValidarAnioPublicacion(anioPublicacion);

        Titulo = titulo.Trim();
        Isbn = isbn?.Trim();
        Editorial = editorial?.Trim();
        AnioPublicacion = anioPublicacion;
        Edicion = edicion?.Trim();
        FechaModificacion = DateTime.UtcNow;
    }

    public void AgregarEjemplar(Ejemplar ejemplar)
    {
        ArgumentNullException.ThrowIfNull(ejemplar);

        bool existeEjemplar = _ejemplares.Any(registro =>
            registro.CodigoInterno == ejemplar.CodigoInterno);

        if (existeEjemplar)
            throw new InvalidOperationException("Ya existe un ejemplar con el mismo código interno.");

        _ejemplares.Add(ejemplar);
        FechaModificacion = DateTime.UtcNow;
    }

    public void QuitarEjemplar(Ejemplar ejemplar)
    {
        ArgumentNullException.ThrowIfNull(ejemplar);

        bool removido = _ejemplares.Remove(ejemplar);

        if (removido)
            FechaModificacion = DateTime.UtcNow;
    }

    public void AgregarAutor(RecursoAutor autor)
    {
        ArgumentNullException.ThrowIfNull(autor);

        bool existeAutor = _autores.Any(registro =>
            registro.AutorId == autor.AutorId);

        if (existeAutor)
            throw new InvalidOperationException("El autor ya está asociado a este recurso bibliográfico.");

        _autores.Add(autor);
        FechaModificacion = DateTime.UtcNow;
    }

    public void QuitarAutor(RecursoAutor autor)
    {
        ArgumentNullException.ThrowIfNull(autor);

        bool removido = _autores.Remove(autor);

        if (removido)
            FechaModificacion = DateTime.UtcNow;
    }

    public void AgregarCategoria(RecursoCategoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        bool existeCategoria = _categorias.Any(registro =>
            registro.CategoriaId == categoria.CategoriaId);

        if (existeCategoria)
            throw new InvalidOperationException("La categoría ya está asociada a este recurso bibliográfico.");

        _categorias.Add(categoria);
        FechaModificacion = DateTime.UtcNow;
    }

    public void QuitarCategoria(RecursoCategoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        bool removida = _categorias.Remove(categoria);

        if (removida)
            FechaModificacion = DateTime.UtcNow;
    }

    private static void ValidarAnioPublicacion(int? anioPublicacion)
    {
        if (!anioPublicacion.HasValue)
            return;

        int anioActual = DateTime.UtcNow.Year;

        if (anioPublicacion.Value < 1450 || anioPublicacion.Value > anioActual)
            throw new ArgumentException($"El año de publicación debe estar entre 1450 y {anioActual}.");
    }
}