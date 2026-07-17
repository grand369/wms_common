namespace Wms.Material.Domain.ValueObjects;

/// <summary>
/// Danger Attribute Value Object (VO-14) — represents the hazard/danger attributes of a material.
/// Stored as JSON column in Material table (nvarchar(max)), nullable (only for hazardous materials).
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public record DangerAttribute
{
    public int DangerLevel { get; init; }
    public string MSDSNumber { get; init; } = string.Empty;
    public string SpecialMark { get; init; } = string.Empty;

    public DangerAttribute() { }

    public DangerAttribute(int dangerLevel = 0, string msdsNumber = "", string specialMark = "")
    {
        DangerLevel = dangerLevel;
        MSDSNumber = msdsNumber ?? string.Empty;
        SpecialMark = specialMark ?? string.Empty;
    }
}
