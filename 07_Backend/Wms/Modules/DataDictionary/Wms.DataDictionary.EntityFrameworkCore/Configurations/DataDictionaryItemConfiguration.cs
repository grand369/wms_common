using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.DataDictionary.Domain.Entities;

namespace Wms.DataDictionary.EntityFrameworkCore.Configurations;

public class DataDictionaryItemConfiguration : IEntityTypeConfiguration<DataDictionaryItem>
{
    public void Configure(EntityTypeBuilder<DataDictionaryItem> builder)
    {
        builder.ToTable("Wms_DataDictionary_DataDictionaryItem");

        builder.Property(x => x.ItemCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ItemValue)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.Dict)
            .WithMany()
            .HasForeignKey(x => x.DictionaryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.DictionaryId, x.ItemCode }).IsUnique();
    }
}
