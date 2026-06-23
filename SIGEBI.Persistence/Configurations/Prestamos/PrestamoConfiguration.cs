using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Prestamos;

public sealed class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
{
    public void Configure(EntityTypeBuilder<Prestamo> builder)
    {
        builder.ToTable("Prestamos", SchemaNames.Prestamos);

        builder.HasKey(prestamo => prestamo.Id);

        builder.Property(prestamo => prestamo.Id)
            .ValueGeneratedOnAdd();

        builder.Property(prestamo => prestamo.UsuarioId)
            .IsRequired();

        builder.Property(prestamo => prestamo.EjemplarId)
            .IsRequired();

        builder.Property(prestamo => prestamo.SolicitudPrestamoId);

        builder.Property(prestamo => prestamo.UsuarioBibliotecarioId)
            .IsRequired();

        builder.Property(prestamo => prestamo.FechaInicio)
            .IsRequired();

        builder.Property(prestamo => prestamo.FechaLimiteDevolucion)
            .IsRequired();

        builder.Property(prestamo => prestamo.FechaDevolucion);

        builder.Property(prestamo => prestamo.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(prestamo => prestamo.FechaCreacion)
            .IsRequired();

        builder.Property(prestamo => prestamo.FechaModificacion);

        builder.Property(prestamo => prestamo.Activo)
            .IsRequired();

        builder.HasIndex(prestamo => prestamo.UsuarioId);

        builder.HasIndex(prestamo => prestamo.EjemplarId);

        builder.HasIndex(prestamo => prestamo.Estado);

        builder.HasIndex(prestamo => prestamo.FechaLimiteDevolucion);

        // Usuario que recibe el préstamo.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(prestamo => prestamo.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ejemplar físico entregado.
        builder.HasOne<Ejemplar>()
            .WithMany()
            .HasForeignKey(prestamo => prestamo.EjemplarId)
            .OnDelete(DeleteBehavior.Restrict);

        // Solicitud que originó el préstamo, si aplica.
        builder.HasOne<SolicitudPrestamo>()
            .WithMany()
            .HasForeignKey(prestamo => prestamo.SolicitudPrestamoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Bibliotecario responsable de formalizar la entrega.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(prestamo => prestamo.UsuarioBibliotecarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(prestamo => prestamo.Devoluciones)
            .WithOne(devolucion => devolucion.Prestamo)
            .HasForeignKey(devolucion => devolucion.PrestamoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}