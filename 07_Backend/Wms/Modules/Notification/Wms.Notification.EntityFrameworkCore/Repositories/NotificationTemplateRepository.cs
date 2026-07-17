using Microsoft.EntityFrameworkCore;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Notification.EntityFrameworkCore.Repositories;

/// <summary>
/// NotificationTemplate Repository Implementation — REP-25
/// </summary>
public class NotificationTemplateRepository : EfCoreRepository<WmsNotificationDbContext, NotificationTemplate, Guid>,
    INotificationTemplateRepository
{
    public NotificationTemplateRepository(IDbContextProvider<WmsNotificationDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<NotificationTemplate?> FindByTemplateNameAsync(string templateName)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(t => t.TemplateName == templateName);
    }

    public async Task<List<NotificationTemplate>> GetByChannelAsync(NotificationChannel channel)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.Channel == channel)
            .OrderBy(t => t.TemplateName)
            .ToListAsync();
    }

    public async Task<List<NotificationTemplate>> GetActiveTemplatesAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.IsActive)
            .OrderBy(t => t.TemplateName)
            .ToListAsync();
    }
}
