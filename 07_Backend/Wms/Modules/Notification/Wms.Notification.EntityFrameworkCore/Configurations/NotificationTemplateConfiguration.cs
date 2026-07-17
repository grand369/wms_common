using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.EntityFrameworkCore.Configurations;

/// <summary>
/// NotificationTemplate EF Core Configuration
/// Table: WmsNotificationTemplates
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("WmsNotificationTemplates");
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => e.TemplateName)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NTT_TemplateName");

        builder.HasIndex(e => e.Channel)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NTT_Channel");

        builder.HasIndex(e => e.IsActive)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NTT_IsActive");

        // SmartEnum conversions
        builder.Property(e => e.TemplateType).IsRequired()
            .HasConversion(t => t.Value, v => NotificationType.FromValue(v));
        builder.Property(e => e.Channel).IsRequired()
            .HasConversion(c => c.Value, v => NotificationChannel.FromValue(v));

        // Property configurations
        builder.Property(e => e.TemplateName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.TemplateContent).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
