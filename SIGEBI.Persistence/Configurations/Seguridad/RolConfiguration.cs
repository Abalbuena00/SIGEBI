using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Seguridad;

public sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles", SchemaNames.Seguridad);

        builder.HasKey(rol => rol.Id);

        builder.Property(rol => rol.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rol => rol.Nombre)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(rol => rol.Descripcion)
            .HasMaxLength(250);

        builder.Property(rol => rol.FechaCreacion)
            .IsRequired();

        builder.Property(rol => rol.FechaModificacion);

        builder.Property(rol => rol.Activo)
            .IsRequired();

        // Evita duplicar roles como Estudiante, Docente o Administrador.
        builder.HasIndex(rol => rol.Nombre)
            .IsUnique();

        // Relación: un Rol puede estar asignado a muchos usuarios.
        builder.HasMany(rol => rol.Usuarios)
            .WithOne(usuarioRol => usuarioRol.Rol)
            .HasForeignKey(usuarioRol => usuarioRol.RolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}