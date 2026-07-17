using Wms.Shared.Domain.Events;

namespace Wms.Inbound.Domain.Events;

/// <summary>
/// DE-008: InboundOrderCreatedEvent — raised when an inbound order is created.
/// Published by InboundOrder aggregate root, subscribed by TaskCenter module.
/// </summary>
public class InboundOrderCreatedEvent : EventDataBase
{
    /// <summary>Inbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Inbound type value (PurchaseReceipt/ProductionReceipt/ReturnReceipt).</summary>
    public int InboundTypeValue { get; set; }

    /// <summary>Target warehouse ID.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Total planned quantity.</summary>
    public decimal TotalPlanQuantity { get; set; }

    /// <summary>Source module — always "Inbound".</summary>
    public string SourceModule { get; set; } = "Inbound";
}
