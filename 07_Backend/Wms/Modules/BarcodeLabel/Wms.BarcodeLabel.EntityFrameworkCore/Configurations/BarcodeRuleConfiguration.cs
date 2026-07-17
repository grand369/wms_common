using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.EntityFrameworkCore.Configurations;

/// <summary>
/// BarcodeRule EF Core Configuration — configures table, indexes, and property mappings.
/// Table: WmsBarcodeRules
/// </summary>
public class BarcodeRuleConfiguration : IEntityTypeConfiguration<BarcodeRule>
{
    public void Configure(EntityTypeBuilder<BarcodeRule> builder)
    {
        builder.ToTable("WmsBarcodeRules");
        builder.HasKey(e => e.Id);

        // Index on RuleName
        builder.HasIndex(e => e.RuleName)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_BarcodeRule_Name");

        // Index on BarcodeType
        builder.HasIndex(e => e.BarcodeType)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_BarcodeRule_Type");

        // Index on IsActive
        builder.HasIndex(e => e.IsActive)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_BarcodeRule_IsActive");

        // Property configurations
        builder.Property(e => e.RuleName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CodePattern).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.SeqCounter).IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Prefix).HasMaxLength(50).IsRequired(false);

        // SmartEnum value conversion: BarcodeType stored as int
        builder.Property(e => e.BarcodeType)
            .HasConversion(
                v => v.Value,
                v => BarcodeType.FromValue(v))
            .IsRequired();

        // SmartEnum value conversion: BarcodeFormat stored as int
        builder.Property(e => e.BarcodeFormat)
            .HasConversion(
                v => v.Value,
                v => BarcodeFormat.FromValue(v))
            .IsRequired();
    }
}
