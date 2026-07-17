using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.CycleCount.Domain.Aggregates;
using Wms.CycleCount.Domain.Enums;
using Wms.CycleCount.Domain.Repositories;
using Wms.Shared.Domain.Interfaces;

namespace Wms.CycleCount.Domain.Services;

/// <summary>
/// DS-07: CycleCountDomainService — domain logic for cycle count operations.
/// Cross-module calls: Inventory (freeze/adjust), TaskCenter (create count task)
/// </summary>
public class CycleCountDomainService : DomainService
{
    private readonly ICycleCountPlanRepository _planRepository;
    private readonly ICycleCountResultRepository _resultRepository;
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly ITaskDomainService _taskDomainService;

    public CycleCountDomainService(
        ICycleCountPlanRepository planRepository,
        ICycleCountResultRepository resultRepository,
        IInventoryDomainService inventoryDomainService,
        ITaskDomainService taskDomainService)
    {
        _planRepository = planRepository;
        _resultRepository = resultRepository;
        _inventoryDomainService = inventoryDomainService;
        _taskDomainService = taskDomainService;
    }

    /// <summary>Create a cycle count plan</summary>
    public async Task<CycleCountPlan> CreatePlanAsync(
        string planNo, CountMethod countMethod, Guid warehouseId, string warehouseCode,
        DateTime plannedDate, bool freezeInventory, decimal threshold, bool blindCount, string? remark)
    {
        if (await _planRepository.FindByNoAsync(planNo) != null)
            throw new BusinessException("Wms.CycleCount:0401", $"Plan no '{planNo}' already exists.");

        return new CycleCountPlan(
            GuidGenerator.Create(), planNo, countMethod, warehouseId, warehouseCode,
            plannedDate, freezeInventory, threshold, blindCount, remark);
    }

    /// <summary>Start counting — freeze inventory if configured (BR-032) + create count task</summary>
    public async Task StartCountingAsync(CycleCountPlan plan)
    {
        plan.StartCounting();

        if (plan.FreezeInventory)
        {
            // TODO: Cross-module call to Inventory to freeze the counted locations
        }

        // Cross-module: create count task
        await _taskDomainService.CreateTaskFromOrderAsync(
            taskTypeValue: 10, // CountTask type
            sourceOrderId: plan.Id,
            sourceOrderType: "CycleCount",
            sourceOrderNo: plan.PlanNo,
            warehouseId: plan.WarehouseId,
            warehouseCode: plan.WarehouseCode);

        await _planRepository.UpdateAsync(plan);
    }

    /// <summary>Generate adjustment from count result — increase/decrease inventory</summary>
    public async Task GenerateAdjustmentAsync(Guid planId)
    {
        var results = await _resultRepository.GetByPlanIdAsync(planId);
        foreach (var result in results)
        {
            if (result.DifferenceQuantity > 0)
            {
                await _inventoryDomainService.IncreaseInventoryAsync(
                    result.MaterialId, Guid.Empty, result.LocationId, null,
                    result.DifferenceQuantity, result.MaterialCode, "", result.LocationCode,
                    "CycleCount", planId);
            }
            else if (result.DifferenceQuantity < 0)
            {
                await _inventoryDomainService.DecreaseInventoryAsync(
                    result.MaterialId, Guid.Empty, result.LocationId, null,
                    0, Math.Abs(result.DifferenceQuantity),
                    "CycleCount", planId, false);
            }
        }
    }
}
