using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        SigebiDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Ejecuta los seeders en orden para dejar la base con datos mínimos.
        await RoleSeeder.SeedAsync(context, cancellationToken);
        await PoliticaPrestamoSeeder.SeedAsync(context, cancellationToken);

        // Confirma en la base de datos todos los registros agregados por los seeders.
        await context.SaveChangesAsync(cancellationToken);
    }
}