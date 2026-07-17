namespace Wms.Outbound.Domain.Enums;

/// <summary>
/// ERP Callback Status Smart Enum — tracks the ERP callback state for outbound orders.
/// Reuses the same values as Inbound module's ErpCallbackStatus.
/// In v1.1, this should be moved to Shared Kernel for reuse across modules.
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
