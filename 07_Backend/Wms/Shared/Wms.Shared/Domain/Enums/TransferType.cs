namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Transfer Type Smart Enum — defines types of transfer operations.
/// Shared across Transfer module.
/// </summary>
public sealed class TransferType : SmartEnum<TransferType, int>
{
    public static readonly TransferType WarehouseTransfer = new TransferType("WarehouseTransfer", 1, "仓间调拨");
    public static readonly TransferType AreaTransfer = new TransferType("AreaTransfer", 2, "库区调拨");
    public static readonly TransferType LocationTransfer = new TransferType("LocationTransfer", 3, "库位调拨");
    public static readonly TransferType CrossCompanyTransfer = new TransferType("CrossCompanyTransfer", 4, "跨公司调拨");

    public string Description { get; }

    private TransferType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
