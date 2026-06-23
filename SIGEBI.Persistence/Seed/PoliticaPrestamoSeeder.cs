using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Enums;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Seed;

public static class PoliticaPrestamoSeeder
{
    public static async Task SeedAsync(
        SigebiDbContext context,
        CancellationToken cancellationToken = default)
    {
        var politicas = new List<PoliticaPrestamo>
        {
            new PoliticaPrestamo(
                tipoMiembro: TipoMiembro.Estudiante,
                maximoPrestamosActivos: 3,
                diasDuracionPrestamo: 7,
                horasReservaTemporal: 24,
                diasSuspensionPorDiaRetraso: 2,
                permitePrestamoConVencidos: false,
                penalizaRetraso: true),

            new PoliticaPrestamo(
                tipoMiembro: TipoMiembro.Docente,
                maximoPrestamosActivos: 10,
                diasDuracionPrestamo: 30,
                horasReservaTemporal: 24,
                diasSuspensionPorDiaRetraso: 1,
                permitePrestamoConVencidos: false,
                penalizaRetraso: true)
        };

        foreach (var politica in politicas)
        {
            // Solo debe existir una política por tipo de miembro.
            var existePolitica = await context.PoliticasPrestamo.AnyAsync(
                registro => registro.TipoMiembro == politica.TipoMiembro,
                cancellationToken);

            if (!existePolitica)
            {
                await context.PoliticasPrestamo.AddAsync(politica, cancellationToken);
            }
        }
    }
}