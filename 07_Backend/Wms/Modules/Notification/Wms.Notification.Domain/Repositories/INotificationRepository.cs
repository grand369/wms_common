using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Domain.Repositories;

/// <summary>
/// INotificationRepository — REP-24
/// </summary>
public interface INotificationRepository : IRepository<NotificationEntity, Guid>
{
    Task<List<NotificationEntity>> GetByRecipientAsync(Guid recipientId, ReadStatus? readStatus = null);
    Task<List<NotificationEntity>> GetByStatusAsync(SendStatus sendStatus);
    Task<int> GetUnreadCountAsync(Guid recipientId);
    Task<NotificationEntity?> GetByCorrelationIdAsync(Guid correlationId);
}
