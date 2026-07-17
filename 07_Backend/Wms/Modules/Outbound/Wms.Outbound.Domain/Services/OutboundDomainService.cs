using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;
using Wms.Outbound.Domain.Events;
using Wms.Outbound.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Helpers;
using Wms.Shared.Domain.Interfaces;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace Wms.Outbound.Domain.Services;

/// <summary>
/// Outbound Domain Service (DS-03) — coordinates outbound order lifecycle operations.
/// 6 core methods: Create, Allocate, ConfirmPicking, ConfirmShipping, ProcessMaterialReturn, Complete.
/// Injected dependencies: IOutboundOrderRepository + IInventoryDomainService + ILocalEventBus.
/// </summary>
public class OutboundDomainService : DomainService
{
    private readonly IOutboundOrderRepository _outboundOrderRepository;
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly ILocalEventBus _localEventBus;

    public OutboundDomainService(
        IOutboundOrderRepository outboundOrderRepository,
        IInventoryDomainService inventoryDomainService,
        ILocalEventBus localEventBus)
    {
        _outboundOrderRepository = outboundOrderRepository;
        _inventoryDomainService = inventoryDomainService;
        _localEventBus = localEventBus;
    }

    /// <summary>
    /// Method 1: CreateOutboundOrder — create and validate a new outbound order.
    /// (DS-03, REQ-OB-001)
    /// </summary>
    public async Task<OutboundOrder> CreateOutboundOrderAsync(
        OutboundType outboundType,
        Guid warehouseId,
        string warehouseCode,
        decimal overIssueRatio,
        bool isEmergency,
        Guid? materialRequisitionId,
        Guid? salesOrderId,
        Guid? returnMaterialOrderId,
        string? remark,
        List<(Guid materialId, string materialCode, string materialName, decimal requiredQuantity, int issueStrategyValue)> lineData)
    {
        var orderId = GuidGenerator.Create();
        var order = new OutboundOrder(
            orderId, outboundType, warehouseId, warehouseCode,
            overIssueRatio, isEmergency,
            materialRequisitionId, salesOrderId, returnMaterialOrderId, remark);

        for (int i = 0; i < lineData.Count; i++)
        {
            var ld = lineData[i];
            order.AddLine(
                GuidGenerator.Create(), i + 1,
                ld.materialId, ld.materialCode, ld.materialName,
                ld.requiredQuantity, ld.issueStrategyValue);
        }

        await _outboundOrderRepository.InsertAsync(order);

        await _localEventBus.PublishAsync(new OutboundOrderCreatedEvent
        {
            AggregateRootId = order.Id,
            OrderId = order.Id,
            OutboundTypeValue = outboundType.Value,
            WarehouseId = warehouseId,
            TotalRequiredQuantity = order.TotalRequiredQuantity,
            SourceModule = "Outbound"
        });

        return order;
    }

    /// <summary>
    /// Method 2: AllocateInventory — allocate inventory for outbound lines.
    /// Calls IInventoryDomainService.ReserveInventoryAsync synchronously for each line.
    /// (DS-03, REQ-OB-001, CROSS-002)
    /// </summary>
    public async Task<OutboundOrder> AllocateInventoryAsync(
        Guid orderId,
        List<(Guid lineId, decimal allocatedQty, Guid locationId, string locationCode)> allocationData)
    {
        var order = await _outboundOrderRepository.GetAsync(orderId);

        // Reserve inventory for each line synchronously (same UoW, CROSS-002)
        foreach (var (lineId, allocatedQty, locationId, locationCode) in allocationData)
        {
            var line = order.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                throw new BusinessException("WMS:Outbound:LineNotFound",
                    $"Outbound line {lineId} not found.");
            }

            // Call Inventory domain service to reserve — synchronous in same UoW
            await _inventoryDomainService.ReserveInventoryAsync(
                line.MaterialId, order.WarehouseId, locationId, line.BatchNumber,
                InventoryStatus.Available.Value,
                allocatedQty, "OutboundOrder", order.Id);

            // Publish allocation event (DE-015)
            await _localEventBus.PublishAsync(new OutboundAllocatedEvent
            {
                AggregateRootId = order.Id,
                OrderId = order.Id,
                LineId = lineId,
                MaterialId = line.MaterialId,
                AllocatedQuantity = allocatedQty,
                LocationId = locationId,
                SourceModule = "Outbound"
            });
        }

        // Transition order status
        order.Allocate(allocationData.Select(a => (a.lineId, a.allocatedQty, (Guid?)a.locationId, (string?)a.locationCode)).ToList());

        await _outboundOrderRepository.UpdateAsync(order);
        return order;
    }

    /// <summary>
    /// Method 3: ConfirmPicking — confirm picking quantities for lines.
    /// (DS-03, REQ-OB-003)
    /// </summary>
    public async Task<OutboundOrder> ConfirmPickingAsync(
        Guid orderId,
        List<(Guid lineId, decimal pickedQty)> pickingData)
    {
        var order = await _outboundOrderRepository.GetAsync(orderId);
        order.ConfirmPicking(pickingData);

        await _outboundOrderRepository.UpdateAsync(order);

        // Publish pick events
        foreach (var (lineId, pickedQty) in pickingData)
        {
            var line = order.Lines.FirstOrDefault(l => l.Id == lineId);
            if (line != null)
            {
                await _localEventBus.PublishAsync(new OutboundPickedEvent
                {
                    AggregateRootId = order.Id,
                    OrderId = order.Id,
                    LineId = lineId,
                    MaterialId = line.MaterialId,
                    PickedQuantity = pickedQty,
                    SourceModule = "Outbound"
                });
            }
        }

        return order;
    }

    /// <summary>
    /// Method 4: ConfirmShipping — confirm shipping and verify quantities.
    /// (DS-03, REQ-OB-007)
    /// </summary>
    public async Task<OutboundOrder> ConfirmShippingAsync(
        Guid orderId,
        List<(Guid lineId, decimal shippedQty)> shippingData)
    {
        var order = await _outboundOrderRepository.GetAsync(orderId);
        order.ConfirmShipping(shippingData);

        await _outboundOrderRepository.UpdateAsync(order);
        return order;
    }

    /// <summary>
    /// Method 5: ProcessMaterialReturn — v1.0 placeholder for material return processing.
    /// (DS-03, REQ-OB-010)
    /// </summary>
    public async Task<OutboundOrder> ProcessMaterialReturnAsync(
        Guid returnId, Guid originalOrderId, decimal returnQty)
    {
        // v1.0 placeholder — material return processing will be expanded in v1.1
        throw new BusinessException("WMS:Outbound:MaterialReturnNotImplemented",
            "Material return processing is not yet implemented in v1.0. Will be available in v1.1.");
    }

    /// <summary>
    /// Method 6: CompleteOutboundOrder — finalize the outbound order.
    /// ⚠️ The caller (AppService) is responsible for synchronously calling:
    /// - IInventoryDomainService.DecreaseInventoryAsync (actual deduction)
    /// - IInventoryDomainService.ReleaseReservationAsync (release reservation)
    /// within the same UoW transaction (CROSS-002).
    /// (DS-03, REQ-OB-008)
    /// </summary>
    public async Task<OutboundOrder> CompleteOutboundOrderAsync(Guid orderId)
    {
        var order = await _outboundOrderRepository.GetAsync(orderId);
        order.Complete();

        await _outboundOrderRepository.UpdateAsync(order);

        await _localEventBus.PublishAsync(new OutboundCompletedEvent
        {
            AggregateRootId = order.Id,
            OrderId = order.Id,
            OutboundTypeValue = order.OutboundType.Value,
            TotalQuantity = order.TotalShippedQuantity,
            SourceModule = "Outbound"
        });

        return order;
    }

    // --- AllocationDomainService (DS-04) methods merged here ---

    /// <summary>
    /// DS-04 Method 1: CheckInventoryAvailability — check if enough inventory is available.
    /// (DS-04, ER-001)
    /// </summary>
    public async Task<InventoryAvailabilityResult> CheckInventoryAvailabilityAsync(
        Guid materialId, Guid warehouseId, decimal reqQty)
    {
        // v1.0 simplified — calls Inventory module to check available quantity
        // In v1.1, will integrate with IInventoryBalanceAppService for actual query
        return new InventoryAvailabilityResult
        {
            IsAvailable = true, // Placeholder
            AvailableQuantity = reqQty, // Placeholder
            MaterialId = materialId,
            WarehouseId = warehouseId
        };
    }

    /// <summary>
    /// DS-04 Method 2: FindAlternativeMaterial — v1.0 placeholder for alternative material recommendation.
    /// (DS-04, ER-001)
    /// </summary>
    public async Task<List<AlternativeMaterialResult>> FindAlternativeMaterialAsync(
        Guid originalMaterialId, Guid warehouseId)
    {
        // v1.0 placeholder — will integrate with Material module in v1.1
        return new List<AlternativeMaterialResult>();
    }
}

/// <summary>
/// Inventory Availability Result — simplified DTO for availability check.
/// </summary>
public class InventoryAvailabilityResult
{
    public bool IsAvailable { get; set; }
    public decimal AvailableQuantity { get; set; }
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
}

/// <summary>
/// Alternative Material Result — placeholder for alternative material recommendation.
/// </summary>
public class AlternativeMaterialResult
{
    public Guid AlternativeMaterialId { get; set; }
    public string AlternativeMaterialCode { get; set; } = string.Empty;
    public decimal AvailableQuantity { get; set; }
    public decimal SubstitutionRatio { get; set; }
}
