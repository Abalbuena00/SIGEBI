using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Penalizaciones;

public sealed class PenalizacionConfiguration : IEntityTypeConfiguration<Penalizacion>
{
    public void Configure(EntityTypeBuilder<Penalizacion> builder)
    {
        builder.ToTable("Penalizaciones", SchemaNames.Penalizaciones);

        builder.HasKey(penalizacion => penalizacion.Id);

        builder.Property(penalizacion => penalizacion.Id)
            .ValueGeneratedOnAdd();

        builder.Property(penalizacion => penalizacion.UsuarioId)
            .IsRequired();

        builder.Property(penalizacion => penalizacion.PrestamoId);

        builder.Property(penalizacion => penalizacion.DiasSuspension)
            .IsRequired();

        builder.Property(penalizacion => penalizacion.FechaInicio)
            .IsRequired();

        builder.Property(penalizacion => penalizacion.FechaFin)
            .IsRequired();

        builder.Property(penalizacion => penalizacion.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(penalizacion => penalizacion.FechaResolucion);

        builder.Property(penalizacion => penalizacion.UsuarioResolutorId);

        builder.Property(penalizacion => penalizacion.MotivoResolucion)
            .HasMaxLength(300);

        builder.Property(penalizacion => penalizacion.FechaCreacion)
            .IsRequired();

        builder.Property(penalizacion => penalizacion.FechaModificacion);

        builder.Property(penalizacion => penalizacion.Activo)
            .IsRequired();

        builder.HasIndex(penalizacion => penalizacion.UsuarioId);

        builder.HasIndex(penalizacion => penalizacion.PrestamoId);

        builder.HasIndex(penalizacion => penalizacion.Estado);

        builder.HasIndex(penalizacion => penalizacion.FechaFin);

        // Usuario penalizado.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(penalizacion => penalizacion.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Préstamo que originó la penalización, si aplica.
        builder.HasOne<Prestamo>()
            .WithMany()
            .HasForeignKey(penalizacion => penalizacion.PrestamoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usuario que resolvió o exoneró la penalización.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(penalizacion => penalizacion.UsuarioResolutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}