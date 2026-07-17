namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Inbound Type Smart Enum — defines types of inbound operations.
/// Shared across Inbound, Production modules.
/// </summary>
public sealed class InboundType : SmartEnum<InboundType, int>
{
    public static readonly InboundType PurchaseReceipt = new InboundType("PurchaseReceipt", 1, "采购入库");
    public static readonly InboundType ProductionReceipt = new InboundType("ProductionReceipt", 2, "生产入库");
    public static readonly InboundType ReturnReceipt = new InboundType("ReturnReceipt", 3, "退货入库");
    public static readonly InboundType TransferInbound = new InboundType("TransferInbound", 4, "调拨入库");

    public string Description { get; }

    private InboundType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
