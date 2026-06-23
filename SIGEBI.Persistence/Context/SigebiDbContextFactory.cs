using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SIGEBI.Persistence.Context;

public sealed class SigebiDbContextFactory : IDesignTimeDbContextFactory<SigebiDbContext>
{
    public SigebiDbContext CreateDbContext(string[] args)
    {
        // Cadena de conexión local usada por EF Core para crear migraciones.
        // Más adelante, cuando creemos la API, moveremos esto a appsettings.json.
        const string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=SIGEBI_DB;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<SigebiDbContext>();

        // Configura SQL Server como proveedor de base de datos para este DbContext.
        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(SigebiDbContext).Assembly.FullName);
            });

        return new SigebiDbContext(optionsBuilder.Options);
    }
}