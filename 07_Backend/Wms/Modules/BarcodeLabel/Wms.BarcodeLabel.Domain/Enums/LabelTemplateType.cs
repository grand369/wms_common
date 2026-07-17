using Wms.Shared.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Enums;

public sealed class LabelTemplateType : SmartEnum<LabelTemplateType, int>
{
    public static readonly LabelTemplateType Inbound = new("Inbound", 0, "入库标签");
    public static readonly LabelTemplateType Outbound = new("Outbound", 1, "出库标签");
    public static readonly LabelTemplateType Product = new("Product", 2, "产品标签");
    public static readonly LabelTemplateType Customer = new("Customer", 3, "客户标签");

    public string Description { get; }

    private LabelTemplateType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
