using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using Wms.Inbound.Domain.Events;
using Wms.Inbound.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Helpers;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace Wms.Inbound.Domain.Services;

/// <summary>
/// Inbound Domain Service (DS-02) — coordinates inbound order lifecycle operations.
/// 6 core methods: Create, ConfirmReceipt, QualityInspection, RecommendPutaway, ConfirmPutaway, Complete.
/// Injected dependencies: IInboundOrderRepository + ILocalEventBus.
/// </summary>
public class InboundDomainService : DomainService
{
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly ILocalEventBus _localEventBus;

    public InboundDomainService(
        IInboundOrderRepository inboundOrderRepository,
        ILocalEventBus localEventBus)
    {
        _inboundOrderRepository = inboundOrderRepository;
        _localEventBus = localEventBus;
    }

    /// <summary>
    /// Method 1: CreateInboundOrder — create and validate a new inbound order.
    /// Generates order number, validates type-specific requirements.
    /// (DS-02, REQ-IN-001)
    /// </summary>
    public async Task<InboundOrder> CreateInboundOrderAsync(
        InboundType inboundType,
        Guid warehouseId,
        string warehouseCode,
        decimal overReceiptRatio,
        bool qualityInspectionRequired,
        Guid? purchaseOrderId,
        string? purchaseOrderNo,
        Guid? productionOrderId,
        Guid? returnOrderId,
        Guid? supplierId,
        string? supplierName,
        string? remark,
        List<(Guid materialId, string materialCode, string materialName, string unit, decimal planQuantity, Guid? putawayWarehouseId, string? putawayWarehouseCode, Guid? putawayAreaId, string? putawayAreaCode, Guid? putawayLocationId, string? putawayLocationCode, string? batchNumber)> lineData)
    {
        var orderId = GuidGenerator.Create();
        var order = new InboundOrder(
            orderId, inboundType, warehouseId, warehouseCode,
            overReceiptRatio, qualityInspectionRequired,
            purchaseOrderId, purchaseOrderNo,
            productionOrderId, returnOrderId,
            supplierId, supplierName, remark);

        // Add lines
        for (int i = 0; i < lineData.Count; i++)
        {
            var ld = lineData[i];
            var line = order.AddLine(
                GuidGenerator.Create(), i + 1,
                ld.materialId, ld.materialCode, ld.materialName, ld.unit,
                ld.planQuantity, ld.batchNumber);
            
            // 设置入库库位信息（如果有）
            if (ld.putawayLocationId.HasValue)
            {
                line.SetPutawayLocation(
                    ld.putawayWarehouseId.Value,
                    ld.putawayWarehouseCode ?? "",
                    ld.putawayAreaId.Value,
                    ld.putawayAreaCode ?? "",
                    ld.putawayLocationId.Value,
                    ld.putawayLocationCode ?? "");
            }
        }

        await _inboundOrderRepository.InsertAsync(order);

        // Publish creation event
        await _localEventBus.PublishAsync(new InboundOrderCreatedEvent
        {
            AggregateRootId = order.Id,
            OrderId = order.Id,
            InboundTypeValue = inboundType.Value,
            WarehouseId = warehouseId,
            TotalPlanQuantity = order.TotalPlanQuantity,
            SourceModule = "Inbound"
        });

        return order;
    }

    /// <summary>
    /// Method 2: ConfirmReceipt — validate received quantities, check over-receipt ratio.
    /// Transitions order from Draft to Confirmed (SM-01).
    /// (DS-02, REQ-IN-005)
    /// </summary>
    public async Task<InboundOrder> ConfirmReceiptAsync(
        Guid orderId,
        List<(Guid lineId, decimal receivedQuantity, string? batchNumber)> recvData)
    {
        var order = await _inboundOrderRepository.GetWithLinesAsync(orderId);

        // Set received quantities on each line
        foreach (var (lineId, receivedQty, batchNo) in recvData)
        {
            order.ReceiveLineQuantity(lineId, receivedQty, batchNo);
        }

        // Transition to Confirmed — validates over-receipt internally
        order.ConfirmReceipt();

        await _inboundOrderRepository.UpdateAsync(order);
        return order;
    }

    /// <summary>
    /// Method 3: ProcessQualityInspection — handle quality inspection result for a line.
    /// Transitions order status based on result (SM-01).
    /// (DS-02, REQ-IN-006)
    /// </summary>
    public async Task<InboundOrder> ProcessQualityInspectionAsync(
        Guid orderId, Guid lineId, QualityStatus result)
    {
        var order = await _inboundOrderRepository.GetWithLinesAsync(orderId);

        if (order.InboundStatus != InboundStatus.Inspecting)
        {
            // Start inspection if still in Confirmed
            if (order.InboundStatus == InboundStatus.Confirmed)
            {
                order.StartQualityInspection();
            }
            else
            {
                throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                    $"Cannot process quality inspection when order status is {order.InboundStatus.Name}. (IN-001)");
            }
        }

        if (result == QualityStatus.Qualified || result == QualityStatus.Skip)
        {
            order.QualityPass(lineId);
        }
        else if (result == QualityStatus.Unqualified)
        {
            order.QualityFail(lineId);
        }

        await _inboundOrderRepository.UpdateAsync(order);
        return order;
    }

    /// <summary>
    /// Method 4: RecommendPutawayLocation — recommend putaway locations for a line.
    /// v1.0 placeholder — will integrate with TaskCenter/RuleEngine in v1.1.
    /// (DS-02, REQ-IN-007)
    /// </summary>
    public async Task<List<RecommendedLocationDto>> RecommendPutawayLocationAsync(
        Guid orderId, Guid lineId)
    {
        // v1.0 placeholder — returns empty list
        // In v1.1, will call TaskCenter/RuleEngine to find optimal locations
        return new List<RecommendedLocationDto>();
    }

    /// <summary>
    /// Method 5: ConfirmPutaway — confirm putaway location for a line.
    /// Transitions line and potentially the order to Completed.
    /// (DS-02, REQ-IN-007)
    /// </summary>
    public async Task<InboundOrder> ConfirmPutawayAsync(
        Guid orderId, Guid lineId, Guid warehouseId, string warehouseCode, Guid areaId, string areaCode, Guid locationId, string locationCode, decimal qty)
    {
        var order = await _inboundOrderRepository.GetWithLinesAsync(orderId);

        // Transition to Putaway if still in Inspecting (all lines passed)
        if (order.InboundStatus == InboundStatus.Inspecting)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                "Cannot confirm putaway when order is still in Inspecting status. All lines must pass quality inspection first.");
        }

        // If still in Confirmed (quality inspection not required), start putaway
        if (order.InboundStatus == InboundStatus.Confirmed)
        {
            order.StartQualityInspection(); // This will transition to Putaway if no inspection needed
        }

        order.ConfirmPutaway(lineId, warehouseId, warehouseCode, areaId, areaCode, locationId, locationCode, qty);

        await _inboundOrderRepository.UpdateAsync(order);
        return order;
    }

    /// <summary>
    /// Method 6: CompleteInboundOrder — finalize the inbound order.
    /// Transitions from Putaway to Completed (SM-01), publishes InboundCompletedEvent.
    /// ⚠️ The caller (AppService) is responsible for synchronously calling
    /// IInventoryDomainService.IncreaseInventoryAsync within the same UoW transaction (CROSS-002).
    /// (DS-02, REQ-IN-001)
    /// </summary>
    public async Task<InboundOrder> CompleteInboundOrderAsync(Guid orderId)
    {
        var order = await _inboundOrderRepository.GetWithLinesAsync(orderId);
        order.Complete();

        await _inboundOrderRepository.UpdateAsync(order);

        // Publish completed event (DE-012) — for Notification/ERP callback
        await _localEventBus.PublishAsync(new InboundCompletedEvent
        {
            AggregateRootId = order.Id,
            OrderId = order.Id,
            InboundTypeValue = order.InboundType.Value,
            TotalQuantity = order.TotalReceivedQuantity,
            SourceModule = "Inbound"
        });

        return order;
    }
}

/// <summary>
/// Recommended Location DTO — placeholder for putaway location recommendation result.
/// Will be expanded in v1.1 when integrating with TaskCenter/RuleEngine.
/// </summary>
public class RecommendedLocationDto
{
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public decimal AvailableCapacity { get; set; }
    public int Priority { get; set; }
}
