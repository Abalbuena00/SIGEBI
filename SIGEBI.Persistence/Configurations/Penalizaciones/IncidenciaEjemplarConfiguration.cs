using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Penalizaciones;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Penalizaciones;

public sealed class IncidenciaEjemplarConfiguration : IEntityTypeConfiguration<IncidenciaEjemplar>
{
    public void Configure(EntityTypeBuilder<IncidenciaEjemplar> builder)
    {
        builder.ToTable("IncidenciasEjemplar", SchemaNames.Penalizaciones);

        builder.HasKey(incidencia => incidencia.Id);

        builder.Property(incidencia => incidencia.Id)
            .ValueGeneratedOnAdd();

        builder.Property(incidencia => incidencia.EjemplarId)
            .IsRequired();

        builder.Property(incidencia => incidencia.PrestamoId);

        builder.Property(incidencia => incidencia.UsuarioReportaId)
            .IsRequired();

        builder.Property(incidencia => incidencia.Tipo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(incidencia => incidencia.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(incidencia => incidencia.FechaRegistro)
            .IsRequired();

        builder.Property(incidencia => incidencia.Cerrada)
            .IsRequired();

        builder.Property(incidencia => incidencia.FechaCierre);

        builder.Property(incidencia => incidencia.UsuarioCierreId);

        builder.Property(incidencia => incidencia.FechaCreacion)
            .IsRequired();

        builder.Property(incidencia => incidencia.FechaModificacion);

        builder.Property(incidencia => incidencia.Activo)
            .IsRequired();

        builder.HasIndex(incidencia => incidencia.EjemplarId);

        builder.HasIndex(incidencia => incidencia.PrestamoId);

        builder.HasIndex(incidencia => incidencia.Tipo);

        builder.HasIndex(incidencia => incidencia.Cerrada);

        // Ejemplar afectado por daño, pérdida u observación.
        builder.HasOne<Ejemplar>()
            .WithMany()
            .HasForeignKey(incidencia => incidencia.EjemplarId)
            .OnDelete(DeleteBehavior.Restrict);

        // Préstamo asociado a la incidencia, si aplica.
        builder.HasOne<Prestamo>()
            .WithMany()
            .HasForeignKey(incidencia => incidencia.PrestamoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usuario que registró la incidencia.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(incidencia => incidencia.UsuarioReportaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usuario que cerró la incidencia.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(incidencia => incidencia.UsuarioCierreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}