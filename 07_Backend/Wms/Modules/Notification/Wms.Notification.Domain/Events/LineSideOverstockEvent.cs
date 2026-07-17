using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Line Side Overstock Event stub — matches Wms.LineSide.Domain.Events.LineSideOverstockEvent
/// </summary>
public class LineSideOverstockEvent : EventDataBase
{
    public Guid LineSideWarehouseId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal CurrentQuantity { get; set; }
    public decimal MaxQuantity { get; set; }
}
