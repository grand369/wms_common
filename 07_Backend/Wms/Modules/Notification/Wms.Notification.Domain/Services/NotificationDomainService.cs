using Volo.Abp.Domain.Services;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Repositories;

namespace Wms.Notification.Domain.Services;

/// <summary>
/// NotificationDomainService — DS-13
/// Core domain service for notification creation, event processing, and batch operations.
/// </summary>
public class NotificationDomainService : DomainService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly INotificationRuleRepository _ruleRepository;

    public NotificationDomainService(
        INotificationRepository notificationRepository,
        INotificationTemplateRepository templateRepository,
        INotificationRuleRepository ruleRepository)
    {
        _notificationRepository = notificationRepository;
        _templateRepository = templateRepository;
        _ruleRepository = ruleRepository;
    }

    /// <summary>
    /// Create a notification aggregate and send it via the appropriate channel.
    /// </summary>
    public async Task<NotificationEntity> CreateAndSendAsync(
        NotificationType notificationType,
        NotificationChannel channel,
        string title,
        string content,
        Guid recipientId,
        string recipientName,
        NotificationPriority? priority = null,
        string? sourceEvent = null,
        string? sourceModule = null,
        Guid? correlationId = null)
    {
        var notification = new NotificationEntity(
            GuidGenerator.Create(),
            notificationType,
            channel,
            title,
            content,
            recipientId,
            recipientName,
            priority ?? NotificationPriority.Normal,
            sourceEvent,
            sourceModule,
            correlationId);

        await _notificationRepository.InsertAsync(notification);

        // Attempt to send — in v1.0 this is a placeholder
        // Future: integrate with actual channel providers (Email, SMS, WeChat, etc.)
        try
        {
            notification.MarkAsSent();
        }
        catch (Exception ex)
        {
            notification.MarkAsFailed(ex.Message);
        }

        await _notificationRepository.UpdateAsync(notification);
        return notification;
    }

    /// <summary>
    /// Process a source event: find matching NotificationRules, resolve templates,
    /// and create notifications for target roles/recipients.
    /// </summary>
    public async Task ProcessEventAsync(string sourceEvent, string sourceModule, Dictionary<string, string> eventData)
    {
        var rules = await _ruleRepository.GetEnabledRulesAsync();
        var matchingRules = rules
            .Where(r => r.SourceEvent == sourceEvent && r.SourceModule == sourceModule)
            .ToList();

        foreach (var rule in matchingRules)
        {
            var title = rule.RuleName;
            var content = $"Event: {sourceEvent} from module {sourceModule}";

            // Resolve template if configured
            if (rule.TemplateId.HasValue)
            {
                var template = await _templateRepository.GetAsync(rule.TemplateId.Value);
                title = template.TemplateName;
                content = template.RenderTemplate(eventData);
            }

            // In v1.0, NotificationRule.TargetRole defines which role receives notifications
            // Future: resolve actual user IDs from role membership
            // For now, create a single notification record as a placeholder
            if (!string.IsNullOrWhiteSpace(rule.TargetRole))
            {
                await CreateAndSendAsync(
                    rule.NotificationType,
                    rule.TargetChannel,
                    title,
                    content,
                    Guid.Empty, // Placeholder — will be resolved from role in future versions
                    rule.TargetRole,
                    rule.Priority,
                    sourceEvent,
                    sourceModule);
            }
        }
    }

    /// <summary>
    /// Batch mark notifications as read for a recipient.
    /// </summary>
    public async Task MarkAsReadBulkAsync(Guid recipientId, List<Guid> notificationIds)
    {
        foreach (var id in notificationIds)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.RecipientId != recipientId)
                throw new BusinessException("WMS:Notification:NotOwnNotification",
                    "不能标记他人的通知为已读。");

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }
    }
}
