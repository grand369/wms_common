using Wms.Shared.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Enums;

public sealed class BarcodeType : SmartEnum<BarcodeType, int>
{
    public static readonly BarcodeType Material = new("Material", 0, "物料条码");
    public static readonly BarcodeType Location = new("Location", 1, "库位条码");
    public static readonly BarcodeType Pallet = new("Pallet", 2, "托盘条码");
    public static readonly BarcodeType Box = new("Box", 3, "箱条码");
    public static readonly BarcodeType Serial = new("Serial", 4, "序列号条码");

    public string Description { get; }

    private BarcodeType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
