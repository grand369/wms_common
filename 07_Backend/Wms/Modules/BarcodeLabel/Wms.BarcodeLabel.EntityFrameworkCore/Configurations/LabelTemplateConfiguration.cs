using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.EntityFrameworkCore.Configurations;

/// <summary>
/// LabelTemplate EF Core Configuration — configures table, indexes, and property mappings.
/// Table: WmsLabelTemplates
/// </summary>
public class LabelTemplateConfiguration : IEntityTypeConfiguration<LabelTemplate>
{
    public void Configure(EntityTypeBuilder<LabelTemplate> builder)
    {
        builder.ToTable("WmsLabelTemplates");
        builder.HasKey(e => e.Id);

        // Index on TemplateName
        builder.HasIndex(e => e.TemplateName)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_LabelTemplate_Name");

        // Index on TemplateType
        builder.HasIndex(e => e.TemplateType)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_LabelTemplate_Type");

        // Index on IsActive
        builder.HasIndex(e => e.IsActive)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_LabelTemplate_IsActive");

        // Property configurations
        builder.Property(e => e.TemplateName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.TemplateContent).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(e => e.TemplateVersion).IsRequired().HasDefaultValue(1);
        builder.Property(e => e.IndustryStandard).HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        // SmartEnum value conversion: LabelTemplateType stored as int
        builder.Property(e => e.TemplateType)
            .HasConversion(
                v => v.Value,
                v => LabelTemplateType.FromValue(v))
            .IsRequired();
    }
}
