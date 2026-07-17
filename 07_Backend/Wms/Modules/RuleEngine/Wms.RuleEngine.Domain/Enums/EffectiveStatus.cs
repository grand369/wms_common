using Wms.Shared.Domain.Enums;

namespace Wms.RuleEngine.Domain.Enums;

/// <summary>
/// EffectiveStatus SmartEnum — defines the effectiveness status of business rules.
/// Active=0, Inactive=1, Draft=2, Archived=3
/// </summary>
public sealed class EffectiveStatus : SmartEnum<EffectiveStatus, int>
{
    public static readonly EffectiveStatus Active = new(0, "Active", "已启用");
    public static readonly EffectiveStatus Inactive = new(1, "Inactive", "已停用");
    public static readonly EffectiveStatus Draft = new(2, "Draft", "草稿");
    public static readonly EffectiveStatus Archived = new(3, "Archived", "已归档");

    public string Description { get; }

    private EffectiveStatus(int value, string name, string description) : base(name, value)
    {
        Description = description;
    }
}
