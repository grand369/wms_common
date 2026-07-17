using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.EntityFrameworkCore.Configurations;

namespace Wms.Notification.EntityFrameworkCore;

/// <summary>
/// Notification Module DbContext — AGG-28/29/30 entity registrations.
/// </summary>
public class WmsNotificationDbContext : AbpDbContext<WmsNotificationDbContext>
{
    // AGG-28: Notifications
    public DbSet<NotificationEntity> Notifications { get; set; }

    // AGG-29: Notification Templates
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    // AGG-30: Notification Rules
    public DbSet<NotificationRule> NotificationRules { get; set; }

    public WmsNotificationDbContext(DbContextOptions<WmsNotificationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all EF Core configurations for Notification module
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new NotificationTemplateConfiguration());
        builder.ApplyConfiguration(new NotificationRuleConfiguration());
    }
}
