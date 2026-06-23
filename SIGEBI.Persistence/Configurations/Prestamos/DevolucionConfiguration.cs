using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Prestamos;

public sealed class DevolucionConfiguration : IEntityTypeConfiguration<Devolucion>
{
    public void Configure(EntityTypeBuilder<Devolucion> builder)
    {
        builder.ToTable("Devoluciones", SchemaNames.Prestamos);

        builder.HasKey(devolucion => devolucion.Id);

        builder.Property(devolucion => devolucion.Id)
            .ValueGeneratedOnAdd();

        builder.Property(devolucion => devolucion.PrestamoId)
            .IsRequired();

        builder.Property(devolucion => devolucion.UsuarioBibliotecarioId)
            .IsRequired();

        builder.Property(devolucion => devolucion.FechaDevolucion)
            .IsRequired();

        builder.Property(devolucion => devolucion.FueTardia)
            .IsRequired();

        builder.Property(devolucion => devolucion.DiasRetraso)
            .IsRequired();

        builder.Property(devolucion => devolucion.Observacion)
            .HasMaxLength(300);

        builder.HasIndex(devolucion => devolucion.PrestamoId);

        builder.HasIndex(devolucion => devolucion.UsuarioBibliotecarioId);

        builder.HasIndex(devolucion => devolucion.FechaDevolucion);

        builder.HasOne(devolucion => devolucion.Prestamo)
            .WithMany(prestamo => prestamo.Devoluciones)
            .HasForeignKey(devolucion => devolucion.PrestamoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Bibliotecario que registró la devolución.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(devolucion => devolucion.UsuarioBibliotecarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}