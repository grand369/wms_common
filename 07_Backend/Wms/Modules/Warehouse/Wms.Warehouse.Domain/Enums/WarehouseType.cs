namespace Wms.Warehouse.Domain.Enums;

/// <summary>
/// Warehouse Type Smart Enum — defines the type/category of a warehouse.
/// At least 12 types covering manufacturing warehouse scenarios.
/// (ENT-01, Phase 3 DDD Design)
/// </summary>
public sealed class WarehouseType : SmartEnum<WarehouseType, int>
{
    public static readonly WarehouseType RawMaterial = new WarehouseType("RawMaterial", 0, "原材料仓");
    public static readonly WarehouseType Finished = new WarehouseType("Finished", 1, "成品仓");
    public static readonly WarehouseType LineSide = new WarehouseType("LineSide", 2, "线边仓");
    public static readonly WarehouseType SemiFinished = new WarehouseType("SemiFinished", 3, "半成品仓");
    public static readonly WarehouseType Auxiliary = new WarehouseType("Auxiliary", 4, "辅料仓");
    public static readonly WarehouseType SparePart = new WarehouseType("SparePart", 5, "备件仓");
    public static readonly WarehouseType Hazardous = new WarehouseType("Hazardous", 6, "危化品仓");
    public static readonly WarehouseType Return = new WarehouseType("Return", 7, "退货仓");
    public static readonly WarehouseType ColdChain = new WarehouseType("ColdChain", 8, "冷链仓");
    public static readonly WarehouseType NormalTemp = new WarehouseType("NormalTemp", 9, "常温仓");
    public static readonly WarehouseType Outdoor = new WarehouseType("Outdoor", 10, "室外仓");
    public static readonly WarehouseType Temporary = new WarehouseType("Temporary", 11, "临时仓");

    public string Description { get; }

    private WarehouseType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
