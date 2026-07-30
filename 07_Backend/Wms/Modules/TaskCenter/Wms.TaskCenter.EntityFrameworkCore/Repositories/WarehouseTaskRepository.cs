using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Shared.Domain.Enums;
using Wms.TaskCenter.Domain.Aggregates;
using Wms.TaskCenter.Domain.Enums;
using TaskStatus = Wms.TaskCenter.Domain.Enums.TaskStatus;
using Wms.TaskCenter.Domain.Repositories;

namespace Wms.TaskCenter.EntityFrameworkCore.Repositories;

/// <summary>
/// WarehouseTaskRepository — REP-10 implementation
/// 8 custom query methods for WarehouseTask aggregate.
/// </summary>
public class WarehouseTaskRepository : EfCoreRepository<WmsTaskCenterDbContext, WarehouseTask, Guid>,
    IWarehouseTaskRepository
{
    public WarehouseTaskRepository(IDbContextProvider<WmsTaskCenterDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<WarehouseTask?> FindByNoAsync(string taskNo)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(t => t.TaskNo == taskNo);
    }

    public async Task<List<WarehouseTask>> GetByWarehouseAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.WarehouseId == warehouseId && !t.IsDeleted)
                          .OrderByDescending(t => t.TaskPriority)
                          .ThenBy(t => t.CreationTime)
                          .ToListAsync();
    }

    public async Task<List<WarehouseTask>> GetByAssignedUserAsync(Guid userId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.AssignedUserId == userId && !t.IsDeleted)
                          .OrderByDescending(t => t.TaskPriority)
                          .ThenBy(t => t.CreationTime)
                          .ToListAsync();
    }

    public async Task<List<WarehouseTask>> GetByStatusAsync(TaskStatus status)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.TaskStatus == status && !t.IsDeleted)
                          .OrderByDescending(t => t.TaskPriority)
                          .ThenBy(t => t.CreationTime)
                          .ToListAsync();
    }

    public async Task<List<WarehouseTask>> GetByPriorityAsync(TaskPriority priority)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.TaskPriority == priority && !t.IsDeleted)
                          .OrderBy(t => t.CreationTime)
                          .ToListAsync();
    }

    public async Task<List<WarehouseTask>> GetPendingAssignmentAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.WarehouseId == warehouseId
                                    && t.TaskStatus == TaskStatus.Created
                                    && !t.IsDeleted)
                          .OrderByDescending(t => t.TaskPriority)
                          .ThenBy(t => t.CreationTime)
                          .ToListAsync();
    }

    public async Task<List<WarehouseTask>> GetTimeoutTasksAsync()
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet.Where(t => t.ExpectedCompletionTime != null
                                    && t.ExpectedCompletionTime < now
                                    && t.TaskStatus != TaskStatus.Completed
                                    && t.TaskStatus != TaskStatus.Cancelled
                                    && !t.IsDeleted)
                          .ToListAsync();
    }

    public async Task<List<WarehouseTask>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(t => t.SourceOrderType == sourceOrderType
                                    && t.SourceOrderId == sourceOrderId
                                    && !t.IsDeleted)
                          .OrderBy(t => t.CreationTime)
                          .ToListAsync();
    }
}
