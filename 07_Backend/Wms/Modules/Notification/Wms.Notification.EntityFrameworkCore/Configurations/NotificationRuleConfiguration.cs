using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.EntityFrameworkCore.Configurations;

/// <summary>
/// NotificationRule EF Core Configuration
/// Table: WmsNotificationRules
/// </summary>
public class NotificationRuleConfiguration : IEntityTypeConfiguration<NotificationRule>
{
    public void Configure(EntityTypeBuilder<NotificationRule> builder)
    {
        builder.ToTable("WmsNotificationRules");
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => new { e.SourceEvent, e.SourceModule })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NTR_SourceEventModule");

        builder.HasIndex(e => e.IsEnabled)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NTR_IsEnabled");

        builder.HasIndex(e => e.TargetChannel)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NTR_TargetChannel");

        // SmartEnum conversions
        builder.Property(e => e.TargetChannel).IsRequired()
            .HasConversion(c => c.Value, v => NotificationChannel.FromValue(v));
        builder.Property(e => e.NotificationType).IsRequired()
            .HasConversion(t => t.Value, v => NotificationType.FromValue(v));
        builder.Property(e => e.Priority).IsRequired()
            .HasConversion(p => p.Value, v => NotificationPriority.FromValue(v));

        // Property configurations
        builder.Property(e => e.RuleName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.SourceEvent).IsRequired().HasMaxLength(200);
        builder.Property(e => e.SourceModule).IsRequired().HasMaxLength(50);
        builder.Property(e => e.TargetRole).HasMaxLength(100);
        builder.Property(e => e.TemplateId).IsRequired(false);
        builder.Property(e => e.IsEnabled).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
