using Wms.Notification.Domain.Aggregates;

namespace Wms.Notification.Domain.Repositories;

/// <summary>
/// INotificationRuleRepository — REP-26
/// </summary>
public interface INotificationRuleRepository : IRepository<NotificationRule, Guid>
{
    Task<List<NotificationRule>> GetBySourceEventAsync(string sourceEvent);
    Task<List<NotificationRule>> GetEnabledRulesAsync();
}
