using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class RecursoAutorConfiguration : IEntityTypeConfiguration<RecursoAutor>
{
    public void Configure(EntityTypeBuilder<RecursoAutor> builder)
    {
        builder.ToTable("RecursosAutores", SchemaNames.Catalogo);

        builder.HasKey(recursoAutor => recursoAutor.Id);

        builder.Property(recursoAutor => recursoAutor.Id)
            .ValueGeneratedOnAdd();

        builder.Property(recursoAutor => recursoAutor.RecursoBibliograficoId)
            .IsRequired();

        builder.Property(recursoAutor => recursoAutor.AutorId)
            .IsRequired();

        // Evita relacionar el mismo autor dos veces con el mismo recurso.
        builder.HasIndex(recursoAutor => new
        {
            recursoAutor.RecursoBibliograficoId,
            recursoAutor.AutorId
        })
            .IsUnique();

        builder.HasOne(recursoAutor => recursoAutor.RecursoBibliografico)
            .WithMany(recurso => recurso.Autores)
            .HasForeignKey(recursoAutor => recursoAutor.RecursoBibliograficoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(recursoAutor => recursoAutor.Autor)
            .WithMany(autor => autor.Recursos)
            .HasForeignKey(recursoAutor => recursoAutor.AutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}