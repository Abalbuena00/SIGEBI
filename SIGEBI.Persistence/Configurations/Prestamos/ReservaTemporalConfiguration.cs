using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Prestamos;

public sealed class ReservaTemporalConfiguration : IEntityTypeConfiguration<ReservaTemporal>
{
    public void Configure(EntityTypeBuilder<ReservaTemporal> builder)
    {
        builder.ToTable("ReservasTemporales", SchemaNames.Prestamos);

        builder.HasKey(reserva => reserva.Id);

        builder.Property(reserva => reserva.Id)
            .ValueGeneratedOnAdd();

        builder.Property(reserva => reserva.SolicitudPrestamoId)
            .IsRequired();

        builder.Property(reserva => reserva.EjemplarId)
            .IsRequired();

        builder.Property(reserva => reserva.FechaInicio)
            .IsRequired();

        builder.Property(reserva => reserva.FechaExpiracion)
            .IsRequired();

        builder.Property(reserva => reserva.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(reserva => reserva.FechaCreacion)
            .IsRequired();

        builder.Property(reserva => reserva.FechaModificacion);

        builder.Property(reserva => reserva.Activo)
            .IsRequired();

        builder.HasIndex(reserva => reserva.SolicitudPrestamoId);

        builder.HasIndex(reserva => reserva.EjemplarId);

        builder.HasIndex(reserva => reserva.Estado);

        builder.HasIndex(reserva => reserva.FechaExpiracion);

        // La reserva nace a partir de una solicitud de préstamo.
        builder.HasOne<SolicitudPrestamo>()
            .WithMany()
            .HasForeignKey(reserva => reserva.SolicitudPrestamoId)
            .OnDelete(DeleteBehavior.Restrict);

        // La reserva bloquea temporalmente un ejemplar físico.
        builder.HasOne<Ejemplar>()
            .WithMany()
            .HasForeignKey(reserva => reserva.EjemplarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}