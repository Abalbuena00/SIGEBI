using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Persistence.Constants;

namespace SIGEBI.Persistence.Configurations.Configuracion;

public sealed class ParametroSistemaConfiguration : IEntityTypeConfiguration<ParametroSistema>
{
    public void Configure(EntityTypeBuilder<ParametroSistema> builder)
    {
        builder.ToTable("ParametrosSistema", SchemaNames.Configuracion);

        builder.HasKey(parametro => parametro.Id);

        builder.Property(parametro => parametro.Id)
            .ValueGeneratedOnAdd();

        builder.Property(parametro => parametro.Clave)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(parametro => parametro.Valor)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(parametro => parametro.Descripcion)
            .HasMaxLength(300);

        builder.Property(parametro => parametro.FechaCreacion)
            .IsRequired();

        builder.Property(parametro => parametro.FechaModificacion);

        builder.Property(parametro => parametro.Activo)
            .IsRequired();

        // La clave funciona como identificador lógico del parámetro.
        builder.HasIndex(parametro => parametro.Clave)
            .IsUnique();
    }
}