using Microsoft.EntityFrameworkCore;

// Importamos las entidades del dominio.
using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Entities.Notificaciones;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Entities.Seguridad;

namespace SIGEBI.Persistence.Context;

// Esta clase representa la sesión de trabajo con la base de datos.
public sealed class SigebiDbContext : DbContext
{
    // Recibe las opciones de configuración:
    // proveedor SQL Server, cadena de conexión, etc.
    public SigebiDbContext(DbContextOptions<SigebiDbContext> options)
        : base(options)
    {
    }

    // Cada DbSet representa una entidad que EF Core podrá guardar en una tabla.
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();

    public DbSet<RecursoBibliografico> RecursosBibliograficos => Set<RecursoBibliografico>();
    public DbSet<Ejemplar> Ejemplares => Set<Ejemplar>();
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<RecursoAutor> RecursosAutores => Set<RecursoAutor>();
    public DbSet<RecursoCategoria> RecursosCategorias => Set<RecursoCategoria>();
    public DbSet<HistorialEstadoEjemplar> HistorialEstadosEjemplar => Set<HistorialEstadoEjemplar>();

    public DbSet<SolicitudPrestamo> SolicitudesPrestamo => Set<SolicitudPrestamo>();
    public DbSet<ReservaTemporal> ReservasTemporales => Set<ReservaTemporal>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();
    public DbSet<Devolucion> Devoluciones => Set<Devolucion>();

    public DbSet<Penalizacion> Penalizaciones => Set<Penalizacion>();
    public DbSet<IncidenciaEjemplar> IncidenciasEjemplar => Set<IncidenciaEjemplar>();

    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    public DbSet<PoliticaPrestamo> PoliticasPrestamo => Set<PoliticaPrestamo>();
    public DbSet<ParametroSistema> ParametrosSistema => Set<ParametroSistema>();

    // Aquí EF Core termina de construir el modelo.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mantiene la configuración base de EF Core.
        base.OnModelCreating(modelBuilder);

        // Aplica automáticamente todos los archivos de configuración
        // que crearemos en la carpeta Configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SigebiDbContext).Assembly);
    }
}