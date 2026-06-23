using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Seguridad;

public sealed class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("UsuariosRoles", SchemaNames.Seguridad);

        builder.HasKey(usuarioRol => usuarioRol.Id);

        builder.Property(usuarioRol => usuarioRol.Id)
            .ValueGeneratedOnAdd();

        builder.Property(usuarioRol => usuarioRol.UsuarioId)
            .IsRequired();

        builder.Property(usuarioRol => usuarioRol.RolId)
            .IsRequired();

        builder.Property(usuarioRol => usuarioRol.FechaAsignacion)
            .IsRequired();

        // Un mismo usuario no debe tener el mismo rol asignado dos veces.
        builder.HasIndex(usuarioRol => new
        {
            usuarioRol.UsuarioId,
            usuarioRol.RolId
        })
            .IsUnique();

        // Relación con Usuario.
        builder.HasOne(usuarioRol => usuarioRol.Usuario)
            .WithMany(usuario => usuario.Roles)
            .HasForeignKey(usuarioRol => usuarioRol.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Rol.
        builder.HasOne(usuarioRol => usuarioRol.Rol)
            .WithMany(rol => rol.Usuarios)
            .HasForeignKey(usuarioRol => usuarioRol.RolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}