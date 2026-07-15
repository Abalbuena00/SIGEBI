namespace SIGEBI.Application.DTOs.Prestamos;

public sealed class PrestamoDto
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int EjemplarId { get; set; }

    public int? SolicitudPrestamoId { get; set; }

    public int UsuarioBibliotecarioId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaLimiteDevolucion { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public int Estado { get; set; }

    public string EstadoDescripcion { get; set; } = string.Empty;

    public bool EstaVencido { get; set; }
}