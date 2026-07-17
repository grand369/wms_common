namespace Wms.Material.Domain.Enums;

/// <summary>
/// Quality Inspection Mode Smart Enum — defines the quality inspection mode for a material.
/// (VO-12, Phase 3 DDD Design)
/// </summary>
public sealed class QualityInspectionMode : SmartEnum<QualityInspectionMode, int>
{
    public static readonly QualityInspectionMode FullInspection = new QualityInspectionMode("FullInspection", 0, "全检");
    public static readonly QualityInspectionMode SamplingInspection = new QualityInspectionMode("SamplingInspection", 1, "抽检");
    public static readonly QualityInspectionMode NoInspection = new QualityInspectionMode("NoInspection", 2, "免检");

    public string Description { get; }

    private QualityInspectionMode(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
