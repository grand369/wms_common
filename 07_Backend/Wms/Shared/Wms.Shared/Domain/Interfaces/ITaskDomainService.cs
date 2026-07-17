namespace Wms.Shared.Domain.Interfaces;

/// <summary>
/// ITaskDomainService — cross-module interface for task center domain operations.
/// Defined in Shared Kernel so that Inbound, Outbound, Transfer, and other modules
/// can inject this interface via DI to call TaskCenter domain methods synchronously
/// within their own UoW transaction (CROSS-003, Phase 6 API Design).
///
/// ⚠️ Return type uses Guid (the created task's ID) to avoid Contracts
/// depending on TaskCenter.Domain types.
/// </summary>
public interface ITaskDomainService
{
    /// <summary>
    /// Create a warehouse task from a source order — e.g. Inbound confirms → Putaway task,
    /// Outbound allocates → Picking task. Called synchronously within the caller's UoW.
    /// </summary>
    Task<Guid> CreateTaskFromOrderAsync(
        int taskTypeValue,
        Guid sourceOrderId,
        string sourceOrderType,
        string sourceOrderNo,
        Guid warehouseId,
        string warehouseCode,
        int priorityValue = 2,
        int assignmentStrategyValue = 0,
        DateTime? expectedCompletionTime = null);

    /// <summary>
    /// Cancel all tasks associated with a source order — e.g. when an inbound/outbound
    /// order is cancelled, its corresponding tasks should also be cancelled.
    /// </summary>
    Task CancelTasksBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId);
}
