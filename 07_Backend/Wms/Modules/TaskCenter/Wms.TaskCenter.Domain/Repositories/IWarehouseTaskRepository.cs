using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Wms.TaskCenter.Domain.Aggregates;
using Wms.TaskCenter.Domain.Enums;
using TaskStatus = Wms.TaskCenter.Domain.Enums.TaskStatus;

namespace Wms.TaskCenter.Domain.Repositories;

/// <summary>
/// IWarehouseTaskRepository — REP-10
/// Custom query methods for WarehouseTask aggregate.
/// </summary>
public interface IWarehouseTaskRepository : IRepository<WarehouseTask, Guid>
{
    Task<WarehouseTask?> FindByNoAsync(string taskNo);
    Task<List<WarehouseTask>> GetByWarehouseAsync(Guid warehouseId);
    Task<List<WarehouseTask>> GetByAssignedUserAsync(Guid userId);
    Task<List<WarehouseTask>> GetByStatusAsync(TaskStatus status);
    Task<List<WarehouseTask>> GetByPriorityAsync(TaskPriority priority);
    Task<List<WarehouseTask>> GetPendingAssignmentAsync(Guid warehouseId);
    Task<List<WarehouseTask>> GetTimeoutTasksAsync();
    Task<List<WarehouseTask>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId);
}
