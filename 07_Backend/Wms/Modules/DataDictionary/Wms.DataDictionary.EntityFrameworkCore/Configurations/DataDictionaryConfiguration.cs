using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.DataDictionary.Domain.Entities;

namespace Wms.DataDictionary.EntityFrameworkCore.Configurations;

public class DataDictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
{
    public void Configure(EntityTypeBuilder<Dictionary> builder)
    {
        builder.ToTable("Wms_DataDictionary_DataDictionary");

        builder.Property(x => x.DictionaryCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DictionaryName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.DictionaryCode).IsUnique();
    }
}
