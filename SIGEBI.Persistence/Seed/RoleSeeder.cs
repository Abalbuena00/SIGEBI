using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        SigebiDbContext context,
        CancellationToken cancellationToken = default)
    {
        var roles = new List<Rol>
        {
            new Rol("Estudiante", "Usuario académico que puede consultar el catálogo y solicitar préstamos."),
            new Rol("Docente", "Usuario académico con políticas de préstamo diferenciadas."),
            new Rol("PersonalBibliotecario", "Usuario interno encargado de gestionar préstamos, devoluciones e inventario."),
            new Rol("Administrador", "Usuario con permisos de administración y configuración del sistema."),
            new Rol("Auditor", "Usuario autorizado para consultar trazabilidad y registros de auditoría.")
        };

        foreach (var rol in roles)
        {
            // Evita insertar roles duplicados si el seeder se ejecuta más de una vez.
            var existeRol = await context.Roles.AnyAsync(
                registro => registro.Nombre == rol.Nombre,
                cancellationToken);

            if (!existeRol)
            {
                await context.Roles.AddAsync(rol, cancellationToken);
            }
        }
    }
}