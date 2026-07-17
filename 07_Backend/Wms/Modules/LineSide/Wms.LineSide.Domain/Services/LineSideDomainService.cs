using System;
using System.Threading.Tasks;
using Wms.LineSide.Domain.Aggregates;
using Wms.LineSide.Domain.Enums;
using Wms.LineSide.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;

namespace Wms.LineSide.Domain.Services;

/// <summary>
/// DS-08: LineSideDomainService — domain logic for line-side operations.
/// Cross-module calls: Inventory (consume/replenish), TaskCenter (create replenishment task)
/// </summary>
public class LineSideDomainService : DomainService
{
    private readonly ILineSideWarehouseRepository _repository;
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly ITaskDomainService _taskDomainService;

    public LineSideDomainService(
        ILineSideWarehouseRepository repository,
        IInventoryDomainService inventoryDomainService,
        ITaskDomainService taskDomainService)
    {
        _repository = repository;
        _inventoryDomainService = inventoryDomainService;
        _taskDomainService = taskDomainService;
    }

    /// <summary>Create a line-side warehouse</summary>
    public async Task<LineSideWarehouse> CreateAsync(
        string code, string name, Guid warehouseId, string warehouseCode,
        Guid productionLineId, string productionLineName,
        Guid? workStationId, ConsumptionMode consumptionMode)
    {
        if (await _repository.FindByCodeAsync(code) != null)
            throw new BusinessException("Wms.LineSide:0401", $"Line-side warehouse code '{code}' already exists.");

        return new LineSideWarehouse(GuidGenerator.Create(), code, name, warehouseId, warehouseCode,
            productionLineId, productionLineName, workStationId, consumptionMode);
    }

    /// <summary>Trigger replenishment — decrease main warehouse + create replenishment task + increase line-side</summary>
    public async Task TriggerReplenishmentAsync(LineSideWarehouse lsw, Guid materialId, decimal replenishQty)
    {
        // Cross-module: decrease main warehouse inventory
        await _inventoryDomainService.DecreaseInventoryAsync(
            materialId, lsw.WarehouseId, Guid.Empty, null, 0, replenishQty,
            "LineSide", lsw.Id, false);

        // Cross-module: create replenishment delivery task
        await _taskDomainService.CreateTaskFromOrderAsync(
            taskTypeValue: 11, // Replenishment type
            sourceOrderId: lsw.Id,
            sourceOrderType: "LineSideReplenishment",
            sourceOrderNo: $"LS-REPLENISH-{lsw.LineSideWarehouseCode}",
            warehouseId: lsw.WarehouseId,
            warehouseCode: lsw.WarehouseCode);

        // Increase line-side stock
        lsw.ReceiveReplenishment(materialId, replenishQty);
        await _repository.UpdateAsync(lsw);
    }
}
