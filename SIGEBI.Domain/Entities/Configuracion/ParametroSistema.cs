using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities.Configuracion;

public sealed class ParametroSistema : AuditableEntity
{
    public string Clave { get; private set; } = string.Empty;

    public string Valor { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    private ParametroSistema()
    {
    }

    public ParametroSistema(
        string clave,
        string valor,
        string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(clave))
            throw new ArgumentException("La clave del parámetro es obligatoria.");

        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("El valor del parámetro es obligatorio.");

        Clave = clave.Trim();
        Valor = valor.Trim();
        Descripcion = descripcion?.Trim();
    }

    public void ActualizarValor(string valor, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("El valor del parámetro es obligatorio.");

        Valor = valor.Trim();

        if (!string.IsNullOrWhiteSpace(descripcion))
            Descripcion = descripcion.Trim();

        FechaModificacion = DateTime.UtcNow;
    }
}