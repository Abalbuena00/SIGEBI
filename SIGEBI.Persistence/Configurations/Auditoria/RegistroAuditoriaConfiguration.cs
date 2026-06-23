using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Auditoria;

public sealed class RegistroAuditoriaConfiguration : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("RegistrosAuditoria", SchemaNames.Auditoria);

        builder.HasKey(registro => registro.Id);

        builder.Property(registro => registro.Id)
            .ValueGeneratedOnAdd();

        builder.Property(registro => registro.UsuarioId);

        builder.Property(registro => registro.Modulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(registro => registro.Accion)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(registro => registro.EntidadAfectada)
            .HasMaxLength(100);

        builder.Property(registro => registro.EntidadAfectadaId);

        builder.Property(registro => registro.Resultado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(registro => registro.Detalle)
            .HasMaxLength(1000);

        builder.Property(registro => registro.DireccionIp)
            .HasMaxLength(60);

        builder.Property(registro => registro.Origen)
            .HasMaxLength(100);

        builder.Property(registro => registro.FechaRegistro)
            .IsRequired();

        builder.HasIndex(registro => registro.UsuarioId);

        builder.HasIndex(registro => registro.Modulo);

        builder.HasIndex(registro => registro.Accion);

        builder.HasIndex(registro => registro.FechaRegistro);

        // La auditoría puede registrar acciones anónimas o fallidas sin usuario.
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(registro => registro.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}