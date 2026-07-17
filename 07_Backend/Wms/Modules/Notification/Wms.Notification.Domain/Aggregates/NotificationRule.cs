using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Domain.Aggregates;

/// <summary>
/// NotificationRule Aggregate Root — AGG-30
/// Maps source events to notification rules (event → notification mapping).
/// </summary>
public class NotificationRule : FullAuditedAggregateRoot<Guid>
{
    public string RuleName { get; private set; }
    public string SourceEvent { get; private set; }
    public string SourceModule { get; private set; }
    public string? TargetRole { get; private set; }
    public NotificationChannel TargetChannel { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public Guid? TemplateId { get; private set; }
    public bool IsEnabled { get; private set; }
    public string? Description { get; private set; }
    public NotificationPriority Priority { get; private set; }

    private NotificationRule() { }

    public NotificationRule(
        Guid id,
        string ruleName,
        string sourceEvent,
        string sourceModule,
        NotificationChannel targetChannel,
        NotificationType notificationType,
        NotificationPriority priority,
        string? targetRole = null,
        Guid? templateId = null,
        bool isEnabled = true,
        string? description = null)
        : base(id)
    {
        RuleName = Check.NotNullOrWhiteSpace(ruleName, nameof(ruleName), maxLength: 100);
        SourceEvent = Check.NotNullOrWhiteSpace(sourceEvent, nameof(sourceEvent), maxLength: 200);
        SourceModule = Check.NotNullOrWhiteSpace(sourceModule, nameof(sourceModule), maxLength: 50);
        TargetChannel = targetChannel ?? NotificationChannel.Internal;
        NotificationType = notificationType ?? throw new ArgumentNullException(nameof(notificationType));
        Priority = priority ?? NotificationPriority.Normal;
        TargetRole = targetRole;
        TemplateId = templateId;
        IsEnabled = isEnabled;
        Description = description;
    }

    public void UpdateTargetChannel(NotificationChannel channel)
    {
        TargetChannel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    public void UpdateTemplate(Guid? templateId)
    {
        TemplateId = templateId;
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }
}
