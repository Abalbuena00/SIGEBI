using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class EjemplarConfiguration : IEntityTypeConfiguration<Ejemplar>
{
    public void Configure(EntityTypeBuilder<Ejemplar> builder)
    {
        builder.ToTable("Ejemplares", SchemaNames.Catalogo);

        builder.HasKey(ejemplar => ejemplar.Id);

        builder.Property(ejemplar => ejemplar.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ejemplar => ejemplar.RecursoBibliograficoId)
            .IsRequired();

        builder.Property(ejemplar => ejemplar.CodigoInterno)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ejemplar => ejemplar.EstadoFisico)
            .HasMaxLength(250);

        builder.Property(ejemplar => ejemplar.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ejemplar => ejemplar.FechaCreacion)
            .IsRequired();

        builder.Property(ejemplar => ejemplar.FechaModificacion);

        builder.Property(ejemplar => ejemplar.Activo)
            .IsRequired();

        // Cada copia física debe tener un código único.
        builder.HasIndex(ejemplar => ejemplar.CodigoInterno)
            .IsUnique();

        builder.HasIndex(ejemplar => ejemplar.RecursoBibliograficoId);

        builder.HasOne(ejemplar => ejemplar.RecursoBibliografico)
            .WithMany(recurso => recurso.Ejemplares)
            .HasForeignKey(ejemplar => ejemplar.RecursoBibliograficoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ejemplar => ejemplar.HistorialEstados)
            .WithOne(historial => historial.Ejemplar)
            .HasForeignKey(historial => historial.EjemplarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}