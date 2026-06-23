using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class AutorConfiguration : IEntityTypeConfiguration<Autor>
{
    public void Configure(EntityTypeBuilder<Autor> builder)
    {
        builder.ToTable("Autores", SchemaNames.Catalogo);

        builder.HasKey(autor => autor.Id);

        builder.Property(autor => autor.Id)
            .ValueGeneratedOnAdd();

        builder.Property(autor => autor.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(autor => autor.FechaCreacion)
            .IsRequired();

        builder.Property(autor => autor.FechaModificacion);

        builder.Property(autor => autor.Activo)
            .IsRequired();

        // Evita duplicar el mismo autor con el mismo nombre.
        builder.HasIndex(autor => autor.Nombre)
            .IsUnique();

        builder.HasMany(autor => autor.Recursos)
            .WithOne(recursoAutor => recursoAutor.Autor)
            .HasForeignKey(recursoAutor => recursoAutor.AutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}