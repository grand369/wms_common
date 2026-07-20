using System.ComponentModel.DataAnnotations;

namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Create DTO — request body for creating a new material.
/// Follows DTO扁平冗余原则: value object properties are flattened.
/// (API-MT-003, Phase 6 API Design)
/// </summary>
public class MaterialCreateDto
{
    [Required] [StringLength(50)] public string MaterialCode { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string MaterialName { get; set; } = string.Empty;
    [StringLength(200)] public string? MaterialNameEn { get; set; }
    public Guid? ClassificationId { get; set; }
    [StringLength(500)] public string? Specification { get; set; }
    [Required] public Guid PrimaryUnitId { get; set; }
    [Required] [StringLength(50)] public string PrimaryUnitName { get; set; } = string.Empty;
    public Guid? SecondaryUnitId { get; set; }
    public decimal? ConversionRate { get; set; }
    
    [StringLength(50)] public string? PurchaseUnitCode { get; set; }
    [StringLength(50)] public string? PurchaseUnitName { get; set; }
    [StringLength(50)] public string? InventoryUnitCode { get; set; }
    [StringLength(50)] public string? InventoryUnitName { get; set; }
    [StringLength(50)] public string? SalesUnitCode { get; set; }
    [StringLength(50)] public string? SalesUnitName { get; set; }
    
    [Required] [Range(0, 7)] public int MaterialType { get; set; }

    // StorageAttribute (VO-11) flattened
    [Range(0, 4)] public int StorageConditionType { get; set; } = 0;
    public int MaxStackingLayers { get; set; } = 1;
    [StringLength(200)] public string PackageSpec { get; set; } = string.Empty;
    public decimal WeightPerUnit { get; set; } = 0;

    // QualityAttribute (VO-12) flattened
    public bool BatchManagementEnabled { get; set; } = false;
    public bool SerialManagementEnabled { get; set; } = false;
    public bool ExpiryManagementEnabled { get; set; } = false;
    public int ShelfLifeDays { get; set; } = 0;
    [Range(0, 2)] public int QualityInspectionMode { get; set; } = 2;

    // InventoryAttribute (VO-13) flattened
    public decimal SafetyStockQuantity { get; set; } = 0;
    public decimal MinOrderQuantity { get; set; } = 0;
    [Range(0, 2)] public int ABCClassification { get; set; } = 2;
    public bool AllowNegativeInventory { get; set; } = false;

    // IssueStrategy (VO-10) flattened
    [Range(0, 3)] public int IssueStrategyType { get; set; } = 0;
    [Range(0, 2)] public int StrategyScope { get; set; } = 0;

    // DangerAttribute (VO-14) flattened (optional)
    [Range(0, 4)] public int DangerLevel { get; set; } = 0;
    [StringLength(50)] public string MSDSNumber { get; set; } = string.Empty;
    [StringLength(200)] public string SpecialMark { get; set; } = string.Empty;

    [Range(0, 3)] public int ErpSyncStatus { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
