namespace Wms.TaskCenter.Domain.Enums;

/// <summary>
/// Assignment Strategy Smart Enum — defines how tasks are assigned to operators.
/// REQ-TC-005: Manual / Region / Skill / LoadBalance
/// </summary>
public sealed class AssignmentStrategy : SmartEnum<AssignmentStrategy, int>
{
    public static readonly AssignmentStrategy Manual = new AssignmentStrategy("Manual", 0, "手动分配");
    public static readonly AssignmentStrategy Region = new AssignmentStrategy("Region", 1, "区域分配");
    public static readonly AssignmentStrategy Skill = new AssignmentStrategy("Skill", 2, "技能分配");
    public static readonly AssignmentStrategy LoadBalance = new AssignmentStrategy("LoadBalance", 3, "负载均衡");

    public string Description { get; }

    private AssignmentStrategy(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
