namespace Wms.Inbound.Domain.Enums;

/// <summary>
/// Inbound Status Smart Enum (SM-01) — 8 states controlling the inbound order lifecycle.
/// Draft → Confirmed → Inspecting/Putaway → Completed → Closed, plus Cancelled and Isolated.
/// </summary>
public sealed class InboundStatus : SmartEnum<InboundStatus, int>
{
    public static readonly InboundStatus Draft = new InboundStatus("Draft", 0, "草稿");
    public static readonly InboundStatus Confirmed = new InboundStatus("Confirmed", 1, "已确认");
    public static readonly InboundStatus Inspecting = new InboundStatus("Inspecting", 2, "质检中");
    public static readonly InboundStatus Isolated = new InboundStatus("Isolated", 3, "隔离");
    public static readonly InboundStatus Putaway = new InboundStatus("Putaway", 4, "上架中");
    public static readonly InboundStatus Completed = new InboundStatus("Completed", 5, "已完成");
    public static readonly InboundStatus Closed = new InboundStatus("Closed", 6, "已关闭");
    public static readonly InboundStatus Cancelled = new InboundStatus("Cancelled", 7, "已取消");

    public string Description { get; }

    private InboundStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
