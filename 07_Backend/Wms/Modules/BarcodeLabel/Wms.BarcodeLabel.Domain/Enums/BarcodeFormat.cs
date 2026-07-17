using Wms.Shared.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Enums;

public sealed class BarcodeFormat : SmartEnum<BarcodeFormat, int>
{
    public static readonly BarcodeFormat QR = new("QR", 0, "QR码");
    public static readonly BarcodeFormat Code128 = new("Code128", 1, "Code128");
    public static readonly BarcodeFormat Code39 = new("Code39", 2, "Code39");
    public static readonly BarcodeFormat EAN13 = new("EAN13", 3, "EAN13");

    public string Description { get; }

    private BarcodeFormat(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
