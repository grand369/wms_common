using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;

namespace Wms.RuleEngine.EntityFrameworkCore.Configurations;

/// <summary>
/// BusinessRuleConfiguration — EF Core table and index configuration for BusinessRule aggregate.
/// Table: WmsBusinessRules. Indexes on RuleName, RuleType, EffectiveStatus, RuleVersion.
/// </summary>
public class BusinessRuleConfiguration : IEntityTypeConfiguration<BusinessRule>
{
    public void Configure(EntityTypeBuilder<BusinessRule> builder)
    {
        builder.ToTable("WmsBusinessRules");
        builder.HasKey(e => e.Id);

        // Index on RuleName
        builder.HasIndex(e => e.RuleName)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Rule_RuleName");

        // Index on RuleType
        builder.HasIndex(e => e.RuleType)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Rule_RuleType");

        // Index on EffectiveStatus
        builder.HasIndex(e => e.EffectiveStatus)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Rule_EffectiveStatus");

        // Index on (RuleType, EffectiveStatus) for efficient rule queries
        builder.HasIndex(e => new { e.RuleType, e.EffectiveStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Rule_TypeStatus");

        // Index on RuleVersion
        builder.HasIndex(e => e.RuleVersion)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_RE_Rule_Version");

        // Index on CreationTime
        builder.HasIndex(e => e.CreationTime)
            .HasName("IDX_RE_Rule_CreationTime");

        // Property configurations
        builder.Property(e => e.RuleName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.RuleType).IsRequired()
            .HasConversion(s => s.Value, v => RuleType.FromValue(v));
        builder.Property(e => e.RuleCondition).IsRequired();
        builder.Property(e => e.RuleAction).IsRequired();
        builder.Property(e => e.RuleVersion).IsRequired();
        builder.Property(e => e.EffectiveStatus).IsRequired()
            .HasConversion(s => s.Value, v => EffectiveStatus.FromValue(v));
        builder.Property(e => e.Description).HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.EffectiveFrom).IsRequired(false);
        builder.Property(e => e.EffectiveTo).IsRequired(false);
        builder.Property(e => e.CreatedByUserId).IsRequired(false);
        builder.Property(e => e.LastModifiedByUserId).IsRequired(false);
    }
}
