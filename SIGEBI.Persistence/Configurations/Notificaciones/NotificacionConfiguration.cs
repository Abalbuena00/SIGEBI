using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Notificaciones;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Notificaciones;

public sealed class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones", SchemaNames.Notificaciones);

        builder.HasKey(notificacion => notificacion.Id);

        builder.Property(notificacion => notificacion.Id)
            .ValueGeneratedOnAdd();

        builder.Property(notificacion => notificacion.UsuarioDestinatarioId)
            .IsRequired();

        builder.Property(notificacion => notificacion.Tipo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(notificacion => notificacion.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(notificacion => notificacion.Titulo)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(notificacion => notificacion.Mensaje)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(notificacion => notificacion.FechaEnvio)
            .IsRequired();

        builder.Property(notificacion => notificacion.FechaLectura);

        builder.Property(notificacion => notificacion.EntidadReferencia)
            .HasMaxLength(100);

        builder.Property(notificacion => notificacion.EntidadReferenciaId);

        builder.Property(notificacion => notificacion.FechaCreacion)
            .IsRequired();

        builder.Property(notificacion => notificacion.FechaModificacion);

        builder.Property(notificacion => notificacion.Activo)
            .IsRequired();

        builder.HasIndex(notificacion => notificacion.UsuarioDestinatarioId);

        builder.HasIndex(notificacion => notificacion.Estado);

        builder.HasIndex(notificacion => notificacion.FechaEnvio);

        // Usuario que recibirá la notificación interna.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(notificacion => notificacion.UsuarioDestinatarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}