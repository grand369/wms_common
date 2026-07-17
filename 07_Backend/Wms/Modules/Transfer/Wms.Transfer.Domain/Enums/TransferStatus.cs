namespace Wms.Transfer.Domain.Enums;

/// <summary>
/// Transfer Status Smart Enum — SM-05 state machine states.
/// Draft → Approved → InTransit → Received → Completed → Closed
/// + Rejected / Cancelled branches
/// </summary>
public sealed class TransferStatus : Wms.Shared.Domain.Enums.SmartEnum<TransferStatus, int>
{
    public static readonly TransferStatus Draft = new TransferStatus("Draft", 0, "草稿");
    public static readonly TransferStatus Approved = new TransferStatus("Approved", 1, "已审批");
    public static readonly TransferStatus Rejected = new TransferStatus("Rejected", 2, "已驳回");
    public static readonly TransferStatus InTransit = new TransferStatus("InTransit", 3, "在途");
    public static readonly TransferStatus Received = new TransferStatus("Received", 4, "已接收");
    public static readonly TransferStatus Completed = new TransferStatus("Completed", 5, "已完成");
    public static readonly TransferStatus Closed = new TransferStatus("Closed", 6, "已关闭");
    public static readonly TransferStatus Cancelled = new TransferStatus("Cancelled", 7, "已取消");

    public string Description { get; }

    private TransferStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
