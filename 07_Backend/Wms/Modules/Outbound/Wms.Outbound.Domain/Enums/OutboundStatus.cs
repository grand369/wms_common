namespace Wms.Outbound.Domain.Enums;

/// <summary>
/// Outbound Status Smart Enum (SM-02) — 7 states controlling the outbound order lifecycle.
/// Draft → Allocated → Picking → Shipped → Completed → Closed, plus Cancelled.
/// </summary>
public sealed class OutboundStatus : SmartEnum<OutboundStatus, int>
{
    public static readonly OutboundStatus Draft = new OutboundStatus("Draft", 0, "草稿");
    public static readonly OutboundStatus Allocated = new OutboundStatus("Allocated", 1, "已分配");
    public static readonly OutboundStatus Picking = new OutboundStatus("Picking", 2, "拣货中");
    public static readonly OutboundStatus Shipped = new OutboundStatus("Shipped", 3, "已发货");
    public static readonly OutboundStatus Completed = new OutboundStatus("Completed", 4, "已完成");
    public static readonly OutboundStatus Closed = new OutboundStatus("Closed", 5, "已关闭");
    public static readonly OutboundStatus Cancelled = new OutboundStatus("Cancelled", 6, "已取消");

    public string Description { get; }

    private OutboundStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
