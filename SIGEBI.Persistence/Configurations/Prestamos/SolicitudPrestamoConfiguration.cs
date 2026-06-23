using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Prestamos;

public sealed class SolicitudPrestamoConfiguration : IEntityTypeConfiguration<SolicitudPrestamo>
{
    public void Configure(EntityTypeBuilder<SolicitudPrestamo> builder)
    {
        builder.ToTable("SolicitudesPrestamo", SchemaNames.Prestamos);

        builder.HasKey(solicitud => solicitud.Id);

        builder.Property(solicitud => solicitud.Id)
            .ValueGeneratedOnAdd();

        builder.Property(solicitud => solicitud.UsuarioId)
            .IsRequired();

        builder.Property(solicitud => solicitud.EjemplarId)
            .IsRequired();

        builder.Property(solicitud => solicitud.FechaSolicitud)
            .IsRequired();

        builder.Property(solicitud => solicitud.FechaExpiracionSolicitud)
            .IsRequired();

        builder.Property(solicitud => solicitud.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(solicitud => solicitud.FechaAprobacion);

        builder.Property(solicitud => solicitud.FechaRechazo);

        builder.Property(solicitud => solicitud.FechaCompletada);

        builder.Property(solicitud => solicitud.UsuarioAprobadorId);

        builder.Property(solicitud => solicitud.MotivoRechazo)
            .HasMaxLength(300);

        builder.Property(solicitud => solicitud.FechaCreacion)
            .IsRequired();

        builder.Property(solicitud => solicitud.FechaModificacion);

        builder.Property(solicitud => solicitud.Activo)
            .IsRequired();

        builder.HasIndex(solicitud => solicitud.UsuarioId);

        builder.HasIndex(solicitud => solicitud.EjemplarId);

        builder.HasIndex(solicitud => solicitud.Estado);

        builder.HasIndex(solicitud => solicitud.FechaExpiracionSolicitud);

        // Relaciona la solicitud con el usuario que pide el préstamo.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(solicitud => solicitud.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relaciona la solicitud con el ejemplar solicitado.
        builder.HasOne<Ejemplar>()
            .WithMany()
            .HasForeignKey(solicitud => solicitud.EjemplarId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usuario interno que aprueba o rechaza la solicitud.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(solicitud => solicitud.UsuarioAprobadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}