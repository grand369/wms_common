using Microsoft.EntityFrameworkCore;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Notification.EntityFrameworkCore.Repositories;

/// <summary>
/// Notification Repository Implementation — REP-24
/// </summary>
public class NotificationRepository : EfCoreRepository<WmsNotificationDbContext, NotificationEntity, Guid>,
    INotificationRepository
{
    public NotificationRepository(IDbContextProvider<WmsNotificationDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<List<NotificationEntity>> GetByRecipientAsync(Guid recipientId, ReadStatus? readStatus = null)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(n => n.RecipientId == recipientId);

        if (readStatus != null)
            query = query.Where(n => n.ReadStatus == readStatus);

        return await query.OrderByDescending(n => n.CreationTime).ToListAsync();
    }

    public async Task<List<NotificationEntity>> GetByStatusAsync(SendStatus sendStatus)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(n => n.SendStatus == sendStatus)
            .OrderByDescending(n => n.CreationTime)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid recipientId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.CountAsync(n => n.RecipientId == recipientId && n.ReadStatus == ReadStatus.Unread);
    }

    public async Task<NotificationEntity?> GetByCorrelationIdAsync(Guid correlationId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(n => n.CorrelationId == correlationId);
    }
}
