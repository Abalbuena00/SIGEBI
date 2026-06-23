using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Seguridad;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios", SchemaNames.Seguridad);

        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Id)
            .ValueGeneratedOnAdd();

        builder.Property(usuario => usuario.NombreCompleto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(usuario => usuario.Correo)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(usuario => usuario.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(usuario => usuario.Matricula)
            .HasMaxLength(50);

        builder.Property(usuario => usuario.NumeroEmpleado)
            .HasMaxLength(50);

        builder.Property(usuario => usuario.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(usuario => usuario.FechaCreacion)
            .IsRequired();

        builder.Property(usuario => usuario.FechaModificacion);

        builder.Property(usuario => usuario.Activo)
            .IsRequired();

        // El correo debe ser único porque será usado como identificador de acceso.
        builder.HasIndex(usuario => usuario.Correo)
            .IsUnique();

        builder.HasIndex(usuario => usuario.Matricula);

        builder.HasIndex(usuario => usuario.NumeroEmpleado);

        // Un usuario puede tener uno o varios roles dentro del sistema.
        builder.HasMany(usuario => usuario.Roles)
            .WithOne(usuarioRol => usuarioRol.Usuario)
            .HasForeignKey(usuarioRol => usuarioRol.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}