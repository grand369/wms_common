using Wms.Shared.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Enums;

public sealed class PrintTaskStatus : SmartEnum<PrintTaskStatus, int>
{
    public static readonly PrintTaskStatus Pending = new("Pending", 0, "待打印");
    public static readonly PrintTaskStatus Printing = new("Printing", 1, "打印中");
    public static readonly PrintTaskStatus Completed = new("Completed", 2, "已完成");
    public static readonly PrintTaskStatus Failed = new("Failed", 3, "打印失败");
    public static readonly PrintTaskStatus Cancelled = new("Cancelled", 4, "已取消");

    public string Description { get; }

    private PrintTaskStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
