using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;

namespace Wms.RuleEngine.EntityFrameworkCore.Configurations;

/// <summary>
/// IndustryPackageConfiguration — EF Core table and index configuration for IndustryPackage aggregate.
/// Table: WmsIndustryPackages. Indexes on PackageName, IndustryType.
/// </summary>
public class IndustryPackageConfiguration : IEntityTypeConfiguration<IndustryPackage>
{
    public void Configure(EntityTypeBuilder<IndustryPackage> builder)
    {
        builder.ToTable("WmsIndustryPackages");
        builder.HasKey(e => e.Id);

        // Index on PackageName
        builder.HasIndex(e => e.PackageName)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Package_PackageName");

        // Index on IndustryType
        builder.HasIndex(e => e.IndustryType)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Package_IndustryType");

        // Index on CreationTime
        builder.HasIndex(e => e.CreationTime)
            .HasName("IDX_RE_Package_CreationTime");

        // Property configurations
        builder.Property(e => e.PackageName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PackageVersion).IsRequired();
        builder.Property(e => e.IndustryType).IsRequired()
            .HasConversion(s => s.Value, v => IndustryType.FromValue(v));
        builder.Property(e => e.PackageContent).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.IsImported).IsRequired();
    }
}
