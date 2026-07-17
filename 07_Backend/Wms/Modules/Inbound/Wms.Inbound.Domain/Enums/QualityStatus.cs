namespace Wms.Inbound.Domain.Enums;

/// <summary>
/// Quality Status Smart Enum — tracks quality inspection result for inbound lines.
/// Pending/Qualified/Unqualified/Skip (4 values).
/// </summary>
public sealed class QualityStatus : SmartEnum<QualityStatus, int>
{
    public static readonly QualityStatus Pending = new QualityStatus("Pending", 0, "待检");
    public static readonly QualityStatus Qualified = new QualityStatus("Qualified", 1, "合格");
    public static readonly QualityStatus Unqualified = new QualityStatus("Unqualified", 2, "不合格");
    public static readonly QualityStatus Skip = new QualityStatus("Skip", 3, "免检");

    public string Description { get; }

    private QualityStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
