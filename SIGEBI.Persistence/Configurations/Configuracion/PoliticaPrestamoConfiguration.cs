using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Configuracion;

public sealed class PoliticaPrestamoConfiguration : IEntityTypeConfiguration<PoliticaPrestamo>
{
    public void Configure(EntityTypeBuilder<PoliticaPrestamo> builder)
    {
        builder.ToTable("PoliticasPrestamo", SchemaNames.Configuracion);

        builder.HasKey(politica => politica.Id);

        builder.Property(politica => politica.Id)
            .ValueGeneratedOnAdd();

        builder.Property(politica => politica.TipoMiembro)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(politica => politica.MaximoPrestamosActivos)
            .IsRequired();

        builder.Property(politica => politica.DiasDuracionPrestamo)
            .IsRequired();

        builder.Property(politica => politica.HorasReservaTemporal)
            .IsRequired();

        builder.Property(politica => politica.DiasSuspensionPorDiaRetraso)
            .IsRequired();

        builder.Property(politica => politica.PermitePrestamoConVencidos)
            .IsRequired();

        builder.Property(politica => politica.PenalizaRetraso)
            .IsRequired();

        builder.Property(politica => politica.FechaCreacion)
            .IsRequired();

        builder.Property(politica => politica.FechaModificacion);

        builder.Property(politica => politica.Activo)
            .IsRequired();

        // Solo debe existir una política activa por tipo de miembro.
        builder.HasIndex(politica => politica.TipoMiembro)
            .IsUnique();
    }
}
