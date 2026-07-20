using Volo.Abp.Domain.Entities.Auditing;
using Wms.Material.Domain.Enums;
using Wms.Material.Domain.Events;
using Wms.Material.Domain.ValueObjects;

namespace Wms.Material.Domain.Aggregates;

/// <summary>
/// Material Aggregate Root (AGG-04, ENT-04) — represents a material with all its attributes.
/// Inherits FullAuditedAggregateRoot for ABP audit fields and soft delete.
/// Contains MaterialSubstituteRelation as child entity (子实体).
/// Value objects StorageAttribute, QualityAttribute, InventoryAttribute, IssueStrategy stored as JSON columns.
/// DangerAttribute is nullable (only for hazardous materials).
/// (Phase 3 DDD Design)
/// </summary>
public class Material : FullAuditedAggregateRoot<Guid>
{
    /// <summary>物料编码（业务自然键，唯一）</summary>
    public string MaterialCode { get; private set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; private set; } = string.Empty;

    /// <summary>物料英文名</summary>
    public string? MaterialNameEn { get; private set; }

    /// <summary>物料分类ID</summary>
    public Guid? ClassificationId { get; private set; }

    /// <summary>规格描述</summary>
    public string? Specification { get; private set; }

    /// <summary>主计量单位ID</summary>
    public Guid PrimaryUnitId { get; private set; }

    /// <summary>主计量单位名称（冗余）</summary>
    public string PrimaryUnitName { get; private set; } = string.Empty;

    /// <summary>辅计量单位ID</summary>
    public Guid? SecondaryUnitId { get; private set; }

    /// <summary>主辅换算率</summary>
    public decimal? ConversionRate { get; private set; }

    /// <summary>采购单位编码（来自SysUnit字典）</summary>
    public string? PurchaseUnitCode { get; private set; }

    /// <summary>采购单位名称（冗余）</summary>
    public string? PurchaseUnitName { get; private set; }

    /// <summary>库存单位编码（来自SysUnit字典）</summary>
    public string? InventoryUnitCode { get; private set; }

    /// <summary>库存单位名称（冗余）</summary>
    public string? InventoryUnitName { get; private set; }

    /// <summary>销售单位编码（来自SysUnit字典）</summary>
    public string? SalesUnitCode { get; private set; }

    /// <summary>销售单位名称（冗余）</summary>
    public string? SalesUnitName { get; private set; }

    /// <summary>物料类型枚举值</summary>
    public int MaterialType { get; private set; }

    /// <summary>仓储属性值对象</summary>
    public StorageAttribute StorageAttribute { get; private set; } = new StorageAttribute();

    /// <summary>质量属性值对象</summary>
    public QualityAttribute QualityAttribute { get; private set; } = new QualityAttribute();

    /// <summary>库存属性值对象</summary>
    public InventoryAttribute InventoryAttribute { get; private set; } = new InventoryAttribute();

    /// <summary>发料策略值对象</summary>
    public IssueStrategy IssueStrategy { get; private set; } = new IssueStrategy();

    /// <summary>危险品属性值对象（可空）</summary>
    public DangerAttribute? DangerAttribute { get; private set; }

    /// <summary>是否启用</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>ERP同步状态枚举值</summary>
    public int ErpSyncStatus { get; private set; }

    /// <summary>替代料关系子实体集合</summary>
    public List<MaterialSubstituteRelation> SubstituteRelations { get; private set; } = new();

    public Material() { }

    /// <summary>
    /// Creates a new Material aggregate root.
    /// </summary>
    public Material(
        Guid id,
        string materialCode,
        string materialName,
        int materialType,
        Guid primaryUnitId,
        string primaryUnitName,
        StorageAttribute storageAttribute,
        QualityAttribute qualityAttribute,
        InventoryAttribute inventoryAttribute,
        IssueStrategy issueStrategy,
        int erpSyncStatus = 0,
        bool isActive = true) : base(id)
    {
        SetMaterialCode(materialCode);
        SetMaterialName(materialName);
        SetType(materialType);
        PrimaryUnitId = primaryUnitId;
        PrimaryUnitName = primaryUnitName ?? throw new ArgumentNullException(nameof(primaryUnitName));
        StorageAttribute = storageAttribute;
        QualityAttribute = qualityAttribute;
        InventoryAttribute = inventoryAttribute;
        IssueStrategy = issueStrategy;
        ErpSyncStatus = erpSyncStatus;
        IsActive = isActive;

        AddLocalEvent(new MaterialCreatedEvent
        {
            MaterialId = Id,
            MaterialCode = MaterialCode,
            MaterialName = MaterialName,
            MaterialType = MaterialType
        });
    }

    public Material SetMaterialCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Material code cannot be empty.", nameof(code));
        MaterialCode = code.Trim();
        return this;
    }

    public Material SetMaterialName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Material name cannot be empty.", nameof(name));
        MaterialName = name.Trim();
        return this;
    }

    /// <summary>Sets the English name for the material.</summary>
    public Material SetMaterialNameEn(string? nameEn)
    {
        MaterialNameEn = nameEn?.Trim();
        return this;
    }

    /// <summary>Sets the classification ID for the material.</summary>
    public Material SetClassificationId(Guid? classificationId)
    {
        ClassificationId = classificationId;
        return this;
    }

    /// <summary>Sets the specification description.</summary>
    public Material SetSpecification(string? specification)
    {
        Specification = specification?.Trim();
        return this;
    }

    /// <summary>Sets the primary unit name (redundant field).</summary>
    public Material SetPrimaryUnitName(string primaryUnitName)
    {
        if (string.IsNullOrWhiteSpace(primaryUnitName))
            throw new ArgumentException("Primary unit name cannot be empty.", nameof(primaryUnitName));
        PrimaryUnitName = primaryUnitName.Trim();
        return this;
    }

    /// <summary>Sets the secondary unit ID and conversion rate.</summary>
    public Material SetSecondaryUnit(Guid? secondaryUnitId, decimal? conversionRate)
    {
        SecondaryUnitId = secondaryUnitId;
        ConversionRate = conversionRate;
        return this;
    }

    /// <summary>Sets the purchase unit.</summary>
    public Material SetPurchaseUnit(string? unitCode, string? unitName)
    {
        PurchaseUnitCode = unitCode?.Trim();
        PurchaseUnitName = unitName?.Trim();
        return this;
    }

    /// <summary>Sets the inventory unit.</summary>
    public Material SetInventoryUnit(string? unitCode, string? unitName)
    {
        InventoryUnitCode = unitCode?.Trim();
        InventoryUnitName = unitName?.Trim();
        return this;
    }

    /// <summary>Sets the sales unit.</summary>
    public Material SetSalesUnit(string? unitCode, string? unitName)
    {
        SalesUnitCode = unitCode?.Trim();
        SalesUnitName = unitName?.Trim();
        return this;
    }

    public Material SetType(int materialType)
    {
        if (!Enums.MaterialType.TryFromValue(materialType, out _))
            throw new ArgumentException($"Invalid material type value: {materialType}", nameof(materialType));
        MaterialType = materialType;
        return this;
    }

    public Material UpdateStorageAttribute(StorageAttribute storageAttribute)
    {
        StorageAttribute = storageAttribute ?? throw new ArgumentNullException(nameof(storageAttribute));
        return this;
    }

    public Material UpdateQualityAttribute(QualityAttribute qualityAttribute)
    {
        QualityAttribute = qualityAttribute ?? throw new ArgumentNullException(nameof(qualityAttribute));
        return this;
    }

    public Material UpdateInventoryAttribute(InventoryAttribute inventoryAttribute)
    {
        InventoryAttribute = inventoryAttribute ?? throw new ArgumentNullException(nameof(inventoryAttribute));
        return this;
    }

    public Material UpdateIssueStrategy(IssueStrategy issueStrategy)
    {
        IssueStrategy = issueStrategy ?? throw new ArgumentNullException(nameof(issueStrategy));
        return this;
    }

    public Material UpdateDangerAttribute(DangerAttribute? dangerAttribute)
    {
        DangerAttribute = dangerAttribute;
        return this;
    }

    /// <summary>
    /// Adds a substitute material relation to this material.
    /// </summary>
    public Material AddSubstituteRelation(Guid substituteMaterialId, string substituteMaterialCode, int priority = 1, decimal ratio = 1.0m)
    {
        if (SubstituteRelations.Any(r => r.SubstituteMaterialId == substituteMaterialId))
            throw new ArgumentException("Substitute material already exists in the relation list.", nameof(substituteMaterialId));

        var relation = new MaterialSubstituteRelation(
            Guid.NewGuid(),
            Id,
            substituteMaterialId,
            substituteMaterialCode,
            priority,
            ratio);

        SubstituteRelations.Add(relation);
        return this;
    }

    /// <summary>
    /// Removes a substitute material relation from this material.
    /// </summary>
    public Material RemoveSubstituteRelation(Guid substituteRelationId)
    {
        var relation = SubstituteRelations.FirstOrDefault(r => r.Id == substituteRelationId);
        if (relation == null)
            throw new ArgumentException("Substitute relation not found.", nameof(substituteRelationId));

        SubstituteRelations.Remove(relation);
        return this;
    }

    public Material SetActive()
    {
        IsActive = true;
        return this;
    }

    public Material Deactivate()
    {
        IsActive = false;
        AddLocalEvent(new MaterialDeactivatedEvent
        {
            MaterialId = Id,
            MaterialCode = MaterialCode
        });
        return this;
    }
}
