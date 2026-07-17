namespace Wms.Material.Domain.ValueObjects;

/// <summary>
/// Quality Attribute Value Object (VO-12) — represents the quality-related attributes of a material.
/// Stored as JSON column in Material table (nvarchar(max)).
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public record QualityAttribute
{
    public bool BatchManagementEnabled { get; init; }
    public bool SerialManagementEnabled { get; init; }
    public bool ExpiryManagementEnabled { get; init; }
    public int ShelfLifeDays { get; init; }
    public int QualityInspectionMode { get; init; }

    public QualityAttribute() { }

    public QualityAttribute(
        bool batchManagementEnabled = false,
        bool serialManagementEnabled = false,
        bool expiryManagementEnabled = false,
        int shelfLifeDays = 0,
        int qualityInspectionMode = 2)
    {
        BatchManagementEnabled = batchManagementEnabled;
        SerialManagementEnabled = serialManagementEnabled;
        ExpiryManagementEnabled = expiryManagementEnabled;
        ShelfLifeDays = shelfLifeDays;
        QualityInspectionMode = qualityInspectionMode;
    }
}
