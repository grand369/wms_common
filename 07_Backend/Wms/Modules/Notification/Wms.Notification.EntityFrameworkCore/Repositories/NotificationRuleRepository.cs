using Microsoft.EntityFrameworkCore;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Notification.EntityFrameworkCore.Repositories;

/// <summary>
/// NotificationRule Repository Implementation — REP-26
/// </summary>
public class NotificationRuleRepository : EfCoreRepository<WmsNotificationDbContext, NotificationRule, Guid>,
    INotificationRuleRepository
{
    public NotificationRuleRepository(IDbContextProvider<WmsNotificationDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<List<NotificationRule>> GetBySourceEventAsync(string sourceEvent)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(r => r.SourceEvent == sourceEvent)
            .OrderBy(r => r.RuleName)
            .ToListAsync();
    }

    public async Task<List<NotificationRule>> GetEnabledRulesAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(r => r.IsEnabled)
            .OrderBy(r => r.RuleName)
            .ToListAsync();
    }
}
