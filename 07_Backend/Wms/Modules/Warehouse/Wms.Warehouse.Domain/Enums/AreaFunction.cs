namespace Wms.Warehouse.Domain.Enums;

/// <summary>
/// Area Function Smart Enum — defines the functional purpose of a warehouse area.
/// (ENT-02, Phase 3 DDD Design)
/// </summary>
public sealed class AreaFunction : SmartEnum<AreaFunction, int>
{
    public static readonly AreaFunction Receiving = new AreaFunction("Receiving", 0, "收货区");
    public static readonly AreaFunction Storage = new AreaFunction("Storage", 1, "存储区");
    public static readonly AreaFunction Shipping = new AreaFunction("Shipping", 2, "发货区");
    public static readonly AreaFunction Isolation = new AreaFunction("Isolation", 3, "隔离区");
    public static readonly AreaFunction QualityInspection = new AreaFunction("QualityInspection", 4, "质检区");
    public static readonly AreaFunction Mixed = new AreaFunction("Mixed", 5, "混合功能区");

    public string Description { get; }

    private AreaFunction(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
