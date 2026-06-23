using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class HistorialEstadoEjemplarConfiguration : IEntityTypeConfiguration<HistorialEstadoEjemplar>
{
    public void Configure(EntityTypeBuilder<HistorialEstadoEjemplar> builder)
    {
        builder.ToTable("HistorialEstadosEjemplar", SchemaNames.Catalogo);

        builder.HasKey(historial => historial.Id);

        builder.Property(historial => historial.Id)
            .ValueGeneratedOnAdd();

        builder.Property(historial => historial.EjemplarId)
            .IsRequired();

        builder.Property(historial => historial.EstadoAnterior)
            .HasConversion<int?>();

        builder.Property(historial => historial.EstadoNuevo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(historial => historial.FechaCambio)
            .IsRequired();

        builder.Property(historial => historial.UsuarioResponsableId);

        builder.Property(historial => historial.Motivo)
            .HasMaxLength(300);

        builder.HasIndex(historial => historial.EjemplarId);

        builder.HasOne(historial => historial.Ejemplar)
            .WithMany(ejemplar => ejemplar.HistorialEstados)
            .HasForeignKey(historial => historial.EjemplarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}