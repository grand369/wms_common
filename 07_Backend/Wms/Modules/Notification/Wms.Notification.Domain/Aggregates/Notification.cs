using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Domain.Aggregates;

/// <summary>
/// Notification Aggregate Root — AGG-28
/// Individual notification record for a recipient.
/// </summary>
public class Notification : FullAuditedAggregateRoot<Guid>
{
    // ── Type ──
    public NotificationType NotificationType { get; private set; }
    public NotificationChannel Channel { get; private set; }

    // ── Content ──
    public string Title { get; private set; }
    public string Content { get; private set; }

    // ── Recipient ──
    public Guid RecipientId { get; private set; }
    public string RecipientName { get; private set; }

    // ── Send ──
    public SendStatus SendStatus { get; private set; }
    public DateTime? SendTime { get; private set; }

    // ── Read ──
    public ReadStatus ReadStatus { get; private set; }
    public DateTime? ReadTime { get; private set; }

    // ── Source ──
    public string? SourceEvent { get; private set; }
    public string? SourceModule { get; private set; }
    public Guid? CorrelationId { get; private set; }

    // ── Retry ──
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }

    // ── Priority ──
    public NotificationPriority Priority { get; private set; }

    // ── EF Core constructor ──
    private Notification() { }

    public Notification(
        Guid id,
        NotificationType notificationType,
        NotificationChannel channel,
        string title,
        string content,
        Guid recipientId,
        string recipientName,
        NotificationPriority priority,
        string? sourceEvent = null,
        string? sourceModule = null,
        Guid? correlationId = null)
        : base(id)
    {
        NotificationType = notificationType ?? throw new ArgumentNullException(nameof(notificationType));
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), maxLength: 200);
        Content = Check.NotNullOrWhiteSpace(content, nameof(content));
        RecipientId = recipientId;
        RecipientName = Check.NotNullOrWhiteSpace(recipientName, nameof(recipientName), maxLength: 100);
        Priority = priority ?? NotificationPriority.Normal;
        SendStatus = SendStatus.Pending;
        ReadStatus = ReadStatus.Unread;
        SourceEvent = sourceEvent;
        SourceModule = sourceModule;
        CorrelationId = correlationId;
        RetryCount = 0;
    }

    public void MarkAsSent()
    {
        if (SendStatus != SendStatus.Pending && SendStatus != SendStatus.Retrying)
            throw new BusinessException("WMS:Notification:InvalidSendStatus",
                "只能在 Pending 或 Retrying 状态下标记为已发送。");

        SendStatus = SendStatus.Sent;
        SendTime = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new BusinessException("WMS:Notification:ErrorMessageRequired",
                "失败原因不能为空。");

        SendStatus = SendStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public void MarkAsRead()
    {
        if (ReadStatus == ReadStatus.Read)
            throw new BusinessException("WMS:Notification:AlreadyRead",
                "该通知已标记为已读。");

        ReadStatus = ReadStatus.Read;
        ReadTime = DateTime.UtcNow;
    }

    public void Retry()
    {
        if (SendStatus != SendStatus.Failed)
            throw new BusinessException("WMS:Notification:RetryNotAllowed",
                "只有发送失败的通知才能重试。");

        SendStatus = SendStatus.Retrying;
        ErrorMessage = null;
    }
}
