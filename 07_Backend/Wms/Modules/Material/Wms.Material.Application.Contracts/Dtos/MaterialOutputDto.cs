namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Output DTO — response body for material queries.
/// Value object properties are flattened following DTO扁平冗余原则.
/// (API-MT-002, Phase 6 API Design)
/// </summary>
public class MaterialOutputDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialNameEn { get; set; }
    public Guid? ClassificationId { get; set; }
    public string? ClassificationName { get; set; }
    public string? Specification { get; set; }
    public Guid PrimaryUnitId { get; set; }
    public string PrimaryUnitName { get; set; } = string.Empty;
    public Guid? SecondaryUnitId { get; set; }
    public string? SecondaryUnitName { get; set; }
    public decimal? ConversionRate { get; set; }
    public int MaterialType { get; set; }
    public string MaterialTypeDescription { get; set; } = string.Empty;

    // StorageAttribute flattened
    public int StorageConditionType { get; set; }
    public string StorageConditionTypeDescription { get; set; } = string.Empty;
    public int MaxStackingLayers { get; set; }
    public string PackageSpec { get; set; } = string.Empty;
    public decimal WeightPerUnit { get; set; }

    // QualityAttribute flattened
    public bool BatchManagementEnabled { get; set; }
    public bool SerialManagementEnabled { get; set; }
    public bool ExpiryManagementEnabled { get; set; }
    public int ShelfLifeDays { get; set; }
    public int QualityInspectionMode { get; set; }
    public string QualityInspectionModeDescription { get; set; } = string.Empty;

    // InventoryAttribute flattened
    public decimal SafetyStockQuantity { get; set; }
    public decimal MinOrderQuantity { get; set; }
    public int ABCClassification { get; set; }
    public string ABCClassificationDescription { get; set; } = string.Empty;
    public bool AllowNegativeInventory { get; set; }

    // IssueStrategy flattened
    public int IssueStrategyType { get; set; }
    public string IssueStrategyTypeDescription { get; set; } = string.Empty;
    public int StrategyScope { get; set; }
    public string StrategyScopeDescription { get; set; } = string.Empty;

    // DangerAttribute flattened (optional)
    public int DangerLevel { get; set; }
    public string DangerLevelDescription { get; set; } = string.Empty;
    public string MSDSNumber { get; set; } = string.Empty;
    public string SpecialMark { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public int ErpSyncStatus { get; set; }
    public string ErpSyncStatusDescription { get; set; } = string.Empty;
    public List<MaterialSubstituteRelationDto> SubstituteRelations { get; set; } = new();
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
}
