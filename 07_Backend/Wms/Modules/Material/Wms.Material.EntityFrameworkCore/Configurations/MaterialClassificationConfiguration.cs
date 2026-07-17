using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Material.Domain.Aggregates;

namespace Wms.Material.EntityFrameworkCore.Configurations;

/// <summary>
/// Material Classification EF Core Configuration.
/// Table: Wms_Material_MaterialClassification
/// (TAB-005, Phase 5 Database Design)
/// </summary>
public class MaterialClassificationConfiguration : IEntityTypeConfiguration<MaterialClassification>
{
    public void Configure(EntityTypeBuilder<MaterialClassification> builder)
    {
        builder.ToTable("Wms_Material_MaterialClassification");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ClassificationCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_MT_ClassificationCode");

        builder.HasIndex(e => e.ParentClassificationId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_MT_Classification_Parent");

        builder.Property(e => e.ClassificationCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.ClassificationName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ClassificationLevel).IsRequired().HasDefaultValue(1);
    }
}
