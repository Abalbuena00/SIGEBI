using SIGEBI.Domain.Base;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities.Configuracion;

public sealed class PoliticaPrestamo : AuditableEntity
{
    public TipoMiembro TipoMiembro { get; private set; }

    public int MaximoPrestamosActivos { get; private set; }

    public int DiasDuracionPrestamo { get; private set; }

    public int HorasReservaTemporal { get; private set; }

    public int DiasSuspensionPorDiaRetraso { get; private set; }

    public bool PermitePrestamoConVencidos { get; private set; }

    public bool PenalizaRetraso { get; private set; }

    private PoliticaPrestamo()
    {
    }

    public PoliticaPrestamo(
        TipoMiembro tipoMiembro,
        int maximoPrestamosActivos,
        int diasDuracionPrestamo,
        int horasReservaTemporal,
        int diasSuspensionPorDiaRetraso,
        bool permitePrestamoConVencidos,
        bool penalizaRetraso)
    {
        if (maximoPrestamosActivos <= 0)
            throw new ArgumentException("El máximo de préstamos activos debe ser mayor que cero.");

        if (diasDuracionPrestamo <= 0)
            throw new ArgumentException("La duración del préstamo debe ser mayor que cero.");

        if (horasReservaTemporal <= 0)
            throw new ArgumentException("La reserva temporal debe ser mayor que cero.");

        if (diasSuspensionPorDiaRetraso < 0)
            throw new ArgumentException("Los días de suspensión no pueden ser negativos.");

        TipoMiembro = tipoMiembro;
        MaximoPrestamosActivos = maximoPrestamosActivos;
        DiasDuracionPrestamo = diasDuracionPrestamo;
        HorasReservaTemporal = horasReservaTemporal;
        DiasSuspensionPorDiaRetraso = diasSuspensionPorDiaRetraso;
        PermitePrestamoConVencidos = permitePrestamoConVencidos;
        PenalizaRetraso = penalizaRetraso;
    }

    public void Actualizar(
        int maximoPrestamosActivos,
        int diasDuracionPrestamo,
        int horasReservaTemporal,
        int diasSuspensionPorDiaRetraso,
        bool permitePrestamoConVencidos,
        bool penalizaRetraso)
    {
        if (maximoPrestamosActivos <= 0)
            throw new ArgumentException("El máximo de préstamos activos debe ser mayor que cero.");

        if (diasDuracionPrestamo <= 0)
            throw new ArgumentException("La duración del préstamo debe ser mayor que cero.");

        if (horasReservaTemporal <= 0)
            throw new ArgumentException("La reserva temporal debe ser mayor que cero.");

        if (diasSuspensionPorDiaRetraso < 0)
            throw new ArgumentException("Los días de suspensión no pueden ser negativos.");

        MaximoPrestamosActivos = maximoPrestamosActivos;
        DiasDuracionPrestamo = diasDuracionPrestamo;
        HorasReservaTemporal = horasReservaTemporal;
        DiasSuspensionPorDiaRetraso = diasSuspensionPorDiaRetraso;
        PermitePrestamoConVencidos = permitePrestamoConVencidos;
        PenalizaRetraso = penalizaRetraso;
        FechaModificacion = DateTime.UtcNow;
    }
}   