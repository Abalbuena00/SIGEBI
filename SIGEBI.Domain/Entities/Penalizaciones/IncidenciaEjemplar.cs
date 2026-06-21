using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Penalizaciones;

public sealed class IncidenciaEjemplar : AuditableEntity
{
    public int EjemplarId { get; private set; }

    public int? PrestamoId { get; private set; }

    public int UsuarioReportaId { get; private set; }

    public TipoIncidenciaEjemplar Tipo { get; private set; }

    public string Descripcion { get; private set; } = string.Empty;

    public DateTime FechaRegistro { get; private set; }

    public bool Cerrada { get; private set; }

    public DateTime? FechaCierre { get; private set; }

    public int? UsuarioCierreId { get; private set; }

    private IncidenciaEjemplar()
    {
    }

    public IncidenciaEjemplar(
        int ejemplarId,
        int usuarioReportaId,
        TipoIncidenciaEjemplar tipo,
        string descripcion,
        int? prestamoId = null)
    {
        if (ejemplarId <= 0)
            throw new ArgumentException("El ejemplar es obligatorio.");

        if (usuarioReportaId <= 0)
            throw new ArgumentException("El usuario que reporta es obligatorio.");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción de la incidencia es obligatoria.");

        EjemplarId = ejemplarId;
        UsuarioReportaId = usuarioReportaId;
        Tipo = tipo;
        Descripcion = descripcion.Trim();
        PrestamoId = prestamoId;
        FechaRegistro = DateTime.UtcNow;
        Cerrada = false;
    }

    public OperationResult Cerrar(int usuarioCierreId)
    {
        if (Cerrada)
            return OperationResult.Failure("La incidencia ya se encuentra cerrada.");

        if (usuarioCierreId <= 0)
            return OperationResult.Failure("El usuario que cierra la incidencia es obligatorio.");

        Cerrada = true;
        UsuarioCierreId = usuarioCierreId;
        FechaCierre = DateTime.UtcNow;
        FechaModificacion = DateTime.UtcNow;

        return OperationResult.Success();
    }
}