namespace Wms.Production.Domain.Enums;

/// <summary>Production Order Status Smart Enum</summary>
public sealed class ProductionOrderStatus : Wms.Shared.Domain.Enums.SmartEnum<ProductionOrderStatus, int>
{
    public static readonly ProductionOrderStatus Planned = new ProductionOrderStatus("Planned", 0, "已计划");
    public static readonly ProductionOrderStatus Released = new ProductionOrderStatus("Released", 1, "已下达");
    public static readonly ProductionOrderStatus InProgress = new ProductionOrderStatus("InProgress", 2, "生产中");
    public static readonly ProductionOrderStatus Completed = new ProductionOrderStatus("Completed", 3, "已完工");
    public static readonly ProductionOrderStatus Closed = new ProductionOrderStatus("Closed", 4, "已关闭");

    public string Description { get; }
    private ProductionOrderStatus(string name, int value, string description) : base(name, value) { Description = description; }
}
