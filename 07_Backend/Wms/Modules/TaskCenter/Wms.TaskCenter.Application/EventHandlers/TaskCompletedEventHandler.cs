using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Wms.TaskCenter.Domain.Events;
using Wms.Outbound.Application.Contracts.Services;
using Wms.Inbound.Application.Contracts.Services;
using Wms.Transfer.Application.Contracts.Services;
using Wms.CycleCount.Application.Contracts.Services;

namespace Wms.TaskCenter.Application.EventHandlers;

/// <summary>
/// DE-031: TaskCompletedEvent handler — orchestrates downstream business logic.
/// 
/// When a warehouse task is completed, this handler:
/// 1. Updates the source business order status (Outbound/Inbound/Transfer/CycleCount)
/// 2. Triggers inventory updates through the respective modules
/// 3. Logs the operation for audit trail
/// 
/// Business Rules:
/// - Picking tasks → Updates outbound order picking status
/// - Shipping tasks → Updates outbound order shipping/complete status
/// - Putaway tasks → Updates inbound order putaway/complete status
/// - Transfer tasks → Updates transfer order complete status
/// - CycleCount tasks → Updates cycle count plan complete status
/// </summary>
public class TaskCompletedEventHandler : ILocalEventHandler<TaskCompletedEvent>, ITransientDependency
{
    private readonly ILogger<TaskCompletedEventHandler> _logger;
    private readonly IOutboundOrderAppService _outboundOrderService;
    private readonly IInboundOrderAppService _inboundOrderService;
    private readonly ITransferOrderAppService _transferOrderService;
    private readonly ICycleCountPlanAppService _cycleCountPlanService;

    public TaskCompletedEventHandler(
        ILogger<TaskCompletedEventHandler> logger,
        IOutboundOrderAppService outboundOrderService,
        IInboundOrderAppService inboundOrderService,
        ITransferOrderAppService transferOrderService,
        ICycleCountPlanAppService cycleCountPlanService)
    {
        _logger = logger;
        _outboundOrderService = outboundOrderService;
        _inboundOrderService = inboundOrderService;
        _transferOrderService = transferOrderService;
        _cycleCountPlanService = cycleCountPlanService;
    }

    [UnitOfWork]
    public async Task HandleEventAsync(TaskCompletedEvent eventData)
    {
        _logger.LogInformation(
            "Processing task completion: TaskId={TaskId}, Type={TaskType}, SourceOrderType={SourceOrderType}, SourceOrderId={SourceOrderId}",
            eventData.TaskId,
            eventData.TaskTypeValue,
            eventData.SourceOrderType,
            eventData.SourceOrderId);

        try
        {
            switch (eventData.SourceOrderType)
            {
                case "OutboundOrder":
                    await HandleOutboundOrderCompletionAsync(eventData);
                    break;

                case "InboundOrder":
                    await HandleInboundOrderCompletionAsync(eventData);
                    break;

                case "TransferOrder":
                    await HandleTransferOrderCompletionAsync(eventData);
                    break;

                case "CycleCountPlan":
                    await HandleCycleCountCompletionAsync(eventData);
                    break;

                default:
                    _logger.LogWarning(
                        "Unknown source order type: {SourceOrderType} for task {TaskId}",
                        eventData.SourceOrderType,
                        eventData.TaskId);
                    break;
            }

            _logger.LogInformation(
                "Task completion processed successfully: TaskId={TaskId}",
                eventData.TaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process task completion: TaskId={TaskId}, Type={TaskType}, SourceOrderType={SourceOrderType}",
                eventData.TaskId,
                eventData.TaskTypeValue,
                eventData.SourceOrderType);
            throw;
        }
    }

    /// <summary>
    /// Handles completion of tasks related to outbound orders.
    /// Picking (2) → Updates picking status
    /// Shipping (8) → Updates shipping status / completes the order
    /// </summary>
    private async Task HandleOutboundOrderCompletionAsync(TaskCompletedEvent eventData)
    {
        _logger.LogInformation(
            "Processing outbound order completion: OrderId={OrderId}, TaskType={TaskType}",
            eventData.SourceOrderId,
            eventData.TaskTypeValue);

        switch (eventData.TaskTypeValue)
        {
            case 2: // Picking (拣货)
                _logger.LogInformation("Picking task completed for outbound order: {OrderId}", eventData.SourceOrderId);
                // After picking task completion, the outbound order picking is confirmed
                // The actual picking quantities should have been recorded during task execution
                // Here we can update the order status if needed
                break;

            case 8: // Shipping (发货)
                _logger.LogInformation("Shipping task completed for outbound order: {OrderId}", eventData.SourceOrderId);
                // After shipping task completion, complete the outbound order
                // This will trigger inventory reduction
                var orderResult = await _outboundOrderService.CompleteAsync(eventData.SourceOrderId);
                _logger.LogInformation(
                    "Outbound order completed: OrderId={OrderId}, Status={Status}",
                    eventData.SourceOrderId,
                    orderResult.OutboundStatusName);
                break;

            case 7: // Packing (打包)
                _logger.LogInformation("Packing task completed for outbound order: {OrderId}", eventData.SourceOrderId);
                // After packing task completion, order is ready for shipping
                break;

            default:
                _logger.LogInformation(
                    "Task type {TaskType} completed for outbound order: {OrderId}",
                    eventData.TaskTypeValue,
                    eventData.SourceOrderId);
                break;
        }
    }

    /// <summary>
    /// Handles completion of tasks related to inbound orders.
    /// Putaway (1) → Updates putaway status / completes the order
    /// QualityInspection (5) → Updates quality inspection status
    /// </summary>
    private async Task HandleInboundOrderCompletionAsync(TaskCompletedEvent eventData)
    {
        _logger.LogInformation(
            "Processing inbound order completion: OrderId={OrderId}, TaskType={TaskType}",
            eventData.SourceOrderId,
            eventData.TaskTypeValue);

        switch (eventData.TaskTypeValue)
        {
            case 1: // Putaway (上架)
                _logger.LogInformation("Putaway task completed for inbound order: {OrderId}", eventData.SourceOrderId);
                // After putaway task completion, complete the inbound order
                // This will trigger inventory increase
                var orderResult = await _inboundOrderService.CompleteAsync(eventData.SourceOrderId);
                _logger.LogInformation(
                    "Inbound order completed: OrderId={OrderId}, Status={Status}",
                    eventData.SourceOrderId,
                    orderResult.InboundStatusName);
                break;

            case 5: // QualityInspection (质检)
                _logger.LogInformation("Quality inspection task completed for inbound order: {OrderId}", eventData.SourceOrderId);
                // After quality inspection, items are approved for putaway
                break;

            default:
                _logger.LogInformation(
                    "Task type {TaskType} completed for inbound order: {OrderId}",
                    eventData.TaskTypeValue,
                    eventData.SourceOrderId);
                break;
        }
    }

    /// <summary>
    /// Handles completion of tasks related to transfer orders.
    /// Transfer (3) → Confirms the transfer order completion
    /// </summary>
    private async Task HandleTransferOrderCompletionAsync(TaskCompletedEvent eventData)
    {
        _logger.LogInformation(
            "Processing transfer order completion: OrderId={OrderId}, TaskType={TaskType}",
            eventData.SourceOrderId,
            eventData.TaskTypeValue);

        switch (eventData.TaskTypeValue)
        {
            case 3: // Transfer (移库)
                _logger.LogInformation("Transfer task completed: OrderId={OrderId}", eventData.SourceOrderId);
                // After transfer task completion, complete the transfer order
                // This will handle inventory movements between warehouses/locations
                var orderResult = await _transferOrderService.CompleteAsync(eventData.SourceOrderId);
                _logger.LogInformation(
                    "Transfer order completed: OrderId={OrderId}",
                    eventData.SourceOrderId);
                break;

            default:
                _logger.LogInformation(
                    "Task type {TaskType} completed for transfer order: {OrderId}",
                    eventData.TaskTypeValue,
                    eventData.SourceOrderId);
                break;
        }
    }

    /// <summary>
    /// Handles completion of tasks related to cycle count plans.
    /// CycleCount (4) → Confirms the cycle count completion
    /// </summary>
    private async Task HandleCycleCountCompletionAsync(TaskCompletedEvent eventData)
    {
        _logger.LogInformation(
            "Processing cycle count completion: PlanId={PlanId}, TaskType={TaskType}",
            eventData.SourceOrderId,
            eventData.TaskTypeValue);

        switch (eventData.TaskTypeValue)
        {
            case 4: // CycleCount (盘点)
                _logger.LogInformation("Cycle count task completed: PlanId={PlanId}", eventData.SourceOrderId);
                // After cycle count task completion, confirm the cycle count
                // This will generate inventory adjustments if there are discrepancies
                var planResult = await _cycleCountPlanService.CompleteAsync(eventData.SourceOrderId);
                _logger.LogInformation(
                    "Cycle count plan completed: PlanId={PlanId}",
                    eventData.SourceOrderId);
                break;

            default:
                _logger.LogInformation(
                    "Task type {TaskType} completed for cycle count plan: {PlanId}",
                    eventData.TaskTypeValue,
                    eventData.SourceOrderId);
                break;
        }
    }
}
