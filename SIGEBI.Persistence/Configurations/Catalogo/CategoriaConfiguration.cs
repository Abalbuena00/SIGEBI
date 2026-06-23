using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias", SchemaNames.Catalogo);

        builder.HasKey(categoria => categoria.Id);

        builder.Property(categoria => categoria.Id)
            .ValueGeneratedOnAdd();

        builder.Property(categoria => categoria.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(categoria => categoria.Descripcion)
            .HasMaxLength(250);

        builder.Property(categoria => categoria.FechaCreacion)
            .IsRequired();

        builder.Property(categoria => categoria.FechaModificacion);

        builder.Property(categoria => categoria.Activo)
            .IsRequired();

        // Evita duplicar categorías con el mismo nombre.
        builder.HasIndex(categoria => categoria.Nombre)
            .IsUnique();

        builder.HasMany(categoria => categoria.Recursos)
            .WithOne(recursoCategoria => recursoCategoria.Categoria)
            .HasForeignKey(recursoCategoria => recursoCategoria.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}