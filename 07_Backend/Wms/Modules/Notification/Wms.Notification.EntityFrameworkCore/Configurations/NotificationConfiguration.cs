using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.EntityFrameworkCore.Configurations;

/// <summary>
/// Notification EF Core Configuration
/// Table: WmsNotifications
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("WmsNotifications");
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => new { e.RecipientId, e.ReadStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NT_RecipientReadStatus");

        builder.HasIndex(e => e.SendStatus)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NT_SendStatus");

        builder.HasIndex(e => e.NotificationType)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NT_NotificationType");

        builder.HasIndex(e => e.CorrelationId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NT_CorrelationId");

        builder.HasIndex(e => e.SendTime)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_NT_SendTime");

        // SmartEnum conversions
        builder.Property(e => e.NotificationType).IsRequired()
            .HasConversion(t => t.Value, v => NotificationType.FromValue(v));
        builder.Property(e => e.Channel).IsRequired()
            .HasConversion(c => c.Value, v => NotificationChannel.FromValue(v));
        builder.Property(e => e.SendStatus).IsRequired()
            .HasConversion(s => s.Value, v => SendStatus.FromValue(v));
        builder.Property(e => e.ReadStatus).IsRequired()
            .HasConversion(r => r.Value, v => ReadStatus.FromValue(v));
        builder.Property(e => e.Priority).IsRequired()
            .HasConversion(p => p.Value, v => NotificationPriority.FromValue(v));

        // Property configurations
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Content).IsRequired();
        builder.Property(e => e.RecipientId).IsRequired();
        builder.Property(e => e.RecipientName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.SendTime).IsRequired(false);
        builder.Property(e => e.ReadTime).IsRequired(false);
        builder.Property(e => e.SourceEvent).HasMaxLength(200);
        builder.Property(e => e.SourceModule).HasMaxLength(50);
        builder.Property(e => e.CorrelationId).IsRequired(false);
        builder.Property(e => e.RetryCount).IsRequired();
        builder.Property(e => e.ErrorMessage).HasMaxLength(2000);
    }
}
