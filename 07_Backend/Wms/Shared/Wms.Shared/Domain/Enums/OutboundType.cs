namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Outbound Type Smart Enum — defines types of outbound operations.
/// Shared across Outbound, Production, LineSide modules.
/// </summary>
public sealed class OutboundType : SmartEnum<OutboundType, int>
{
    public static readonly OutboundType MaterialRequisition = new OutboundType("MaterialRequisition", 1, "领料出库");
    public static readonly OutboundType SalesShipment = new OutboundType("SalesShipment", 2, "销售出库");
    public static readonly OutboundType ReturnMaterial = new OutboundType("ReturnMaterial", 3, "退料出库");
    public static readonly OutboundType TransferOutbound = new OutboundType("TransferOutbound", 4, "调拨出库");

    public string Description { get; }

    private OutboundType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
