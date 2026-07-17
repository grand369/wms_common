namespace Wms.Inbound.Domain.Enums;

/// <summary>
/// ERP Callback Status Smart Enum — tracks the ERP callback state for inbound orders.
/// None/Success/Failed/Pending (4 values).
/// Shared with Outbound module — if Outbound also needs this enum, it should reference
/// this or we move it to Shared Kernel in v1.1.
/// </summary>
public sealed class ErpCallbackStatus : SmartEnum<ErpCallbackStatus, int>
{
    public static readonly ErpCallbackStatus None = new ErpCallbackStatus("None", 0, "未回传");
    public static readonly ErpCallbackStatus Success = new ErpCallbackStatus("Success", 1, "回传成功");
    public static readonly ErpCallbackStatus Failed = new ErpCallbackStatus("Failed", 2, "回传失败");
    public static readonly ErpCallbackStatus Pending = new ErpCallbackStatus("Pending", 3, "回传中");

    public string Description { get; }

    private ErpCallbackStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
