namespace Wms.CycleCount.Domain.Enums;

/// <summary>
/// Count Method Smart Enum — full/cycle/spot counting methods
/// </summary>
public sealed class CountMethod : Wms.Shared.Domain.Enums.SmartEnum<CountMethod, int>
{
    public static readonly CountMethod Full = new CountMethod("Full", 1, "全盘");
    public static readonly CountMethod Cycle = new CountMethod("Cycle", 2, "循环盘点");
    public static readonly CountMethod Spot = new CountMethod("Spot", 3, "抽盘");

    public string Description { get; }
    private CountMethod(string name, int value, string description) : base(name, value) { Description = description; }
}
