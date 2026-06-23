using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Catalogo;

public sealed class RecursoBibliograficoConfiguration : IEntityTypeConfiguration<RecursoBibliografico>
{
    public void Configure(EntityTypeBuilder<RecursoBibliografico> builder)
    {
        builder.ToTable("RecursosBibliograficos", SchemaNames.Catalogo);

        builder.HasKey(recurso => recurso.Id);

        builder.Property(recurso => recurso.Id)
            .ValueGeneratedOnAdd();

        builder.Property(recurso => recurso.CodigoInterno)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(recurso => recurso.Titulo)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(recurso => recurso.Isbn)
            .HasMaxLength(30);

        builder.Property(recurso => recurso.Editorial)
            .HasMaxLength(150);

        builder.Property(recurso => recurso.AnioPublicacion);

        builder.Property(recurso => recurso.Edicion)
            .HasMaxLength(80);

        builder.Property(recurso => recurso.FechaCreacion)
            .IsRequired();

        builder.Property(recurso => recurso.FechaModificacion);

        builder.Property(recurso => recurso.Activo)
            .IsRequired();

        // Código interno único para identificar recursos sin depender del ISBN.
        builder.HasIndex(recurso => recurso.CodigoInterno)
            .IsUnique();

        // El ISBN es opcional, pero si existe no debe repetirse.
        builder.HasIndex(recurso => recurso.Isbn)
            .IsUnique()
            .HasFilter("[Isbn] IS NOT NULL");

        builder.HasIndex(recurso => recurso.Titulo);

        builder.HasMany(recurso => recurso.Ejemplares)
            .WithOne(ejemplar => ejemplar.RecursoBibliografico)
            .HasForeignKey(ejemplar => ejemplar.RecursoBibliograficoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(recurso => recurso.Autores)
            .WithOne(recursoAutor => recursoAutor.RecursoBibliografico)
            .HasForeignKey(recursoAutor => recursoAutor.RecursoBibliograficoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(recurso => recurso.Categorias)
            .WithOne(recursoCategoria => recursoCategoria.RecursoBibliografico)
            .HasForeignKey(recursoCategoria => recursoCategoria.RecursoBibliograficoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}