using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class RecursoCategoriaConfiguration : IEntityTypeConfiguration<RecursoCategoria>
{
    public void Configure(EntityTypeBuilder<RecursoCategoria> builder)
    {
        builder.ToTable("RecursosCategorias", SchemaNames.Catalogo);

        builder.HasKey(recursoCategoria => recursoCategoria.Id);

        builder.Property(recursoCategoria => recursoCategoria.Id)
            .ValueGeneratedOnAdd();

        builder.Property(recursoCategoria => recursoCategoria.RecursoBibliograficoId)
            .IsRequired();

        builder.Property(recursoCategoria => recursoCategoria.CategoriaId)
            .IsRequired();

        // Evita duplicar la misma categoría para el mismo recurso.
        builder.HasIndex(recursoCategoria => new
        {
            recursoCategoria.RecursoBibliograficoId,
            recursoCategoria.CategoriaId
        })
            .IsUnique();

        builder.HasOne(recursoCategoria => recursoCategoria.RecursoBibliografico)
            .WithMany(recurso => recurso.Categorias)
            .HasForeignKey(recursoCategoria => recursoCategoria.RecursoBibliograficoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(recursoCategoria => recursoCategoria.Categoria)
            .WithMany(categoria => categoria.Recursos)
            .HasForeignKey(recursoCategoria => recursoCategoria.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}