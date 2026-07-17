namespace Wms.Production.Domain.Enums;

/// <summary>Requisition Status Smart Enum</summary>
public sealed class RequisitionStatus : Wms.Shared.Domain.Enums.SmartEnum<RequisitionStatus, int>
{
    public static readonly RequisitionStatus Draft = new RequisitionStatus("Draft", 0, "草稿");
    public static readonly RequisitionStatus Submitted = new RequisitionStatus("Submitted", 1, "已提交");
    public static readonly RequisitionStatus Issued = new RequisitionStatus("Issued", 2, "已发料");
    public static readonly RequisitionStatus PartiallyIssued = new RequisitionStatus("PartiallyIssued", 3, "部分发料");
    public static readonly RequisitionStatus Completed = new RequisitionStatus("Completed", 4, "已完成");
    public static readonly RequisitionStatus Cancelled = new RequisitionStatus("Cancelled", 5, "已取消");

    public string Description { get; }
    private RequisitionStatus(string name, int value, string description) : base(name, value) { Description = description; }
}
