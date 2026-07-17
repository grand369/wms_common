using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Domain.Repositories;

/// <summary>
/// INotificationTemplateRepository — REP-25
/// </summary>
public interface INotificationTemplateRepository : IRepository<NotificationTemplate, Guid>
{
    Task<NotificationTemplate?> FindByTemplateNameAsync(string templateName);
    Task<List<NotificationTemplate>> GetByChannelAsync(NotificationChannel channel);
    Task<List<NotificationTemplate>> GetActiveTemplatesAsync();
}
