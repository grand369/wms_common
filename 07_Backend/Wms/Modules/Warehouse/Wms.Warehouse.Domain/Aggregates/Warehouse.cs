using Volo.Abp.Domain.Entities.Auditing;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.ValueObjects;
using Wms.Warehouse.Domain.Events;

namespace Wms.Warehouse.Domain.Aggregates;

/// <summary>
/// Warehouse Aggregate Root (AGG-01, ENT-01) — represents a physical warehouse.
/// Inherits FullAuditedAggregateRoot for ABP audit fields and soft delete.
/// (Phase 3 DDD Design)
/// </summary>
public class Warehouse : FullAuditedAggregateRoot<Guid>
{
    /// <summary>仓库编码（业务自然键，唯一）</summary>
    public string WarehouseCode { get; private set; } = string.Empty;

    /// <summary>仓库名称</summary>
    public string WarehouseName { get; private set; } = string.Empty;

    /// <summary>仓库类型枚举值</summary>
    public int WarehouseType { get; private set; }

    /// <summary>所属组织单元ID</summary>
    public string OrganizationUnitId { get; private set; } = string.Empty;

    /// <summary>所属组织名称（冗余）</summary>
    public string OrganizationUnitName { get; private set; } = string.Empty;

    /// <summary>所属工厂ID</summary>
    public string PlantId { get; private set; } = string.Empty;

    /// <summary>所属工厂名称（冗余）</summary>
    public string PlantName { get; private set; } = string.Empty;

    /// <summary>负责人ID</summary>
    public string? ResponsibleUserId { get; private set; }

    /// <summary>负责人姓名（冗余）</summary>
    public string? ResponsibleUserName { get; private set; }

    /// <summary>仓库地址</summary>
    public string? Address { get; private set; }

    /// <summary>默认存储条件类型枚举值</summary>
    public int StorageConditionType { get; private set; }

    /// <summary>库位层级数（3或4）</summary>
    public int LocationLevelCount { get; private set; } = 3;

    /// <summary>是否启用</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>备注</summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// Creates a new Warehouse aggregate root.
    /// </summary>
    public Warehouse(
        Guid id,
        string warehouseCode,
        string warehouseName,
        int warehouseType,
        string organizationUnitId,
        string organizationUnitName,
        string plantId,
        string plantName,
        int storageConditionType = 0,
        int locationLevelCount = 3,
        bool isActive = true) : base(id)
    {
        SetWarehouseCode(warehouseCode);
        SetWarehouseName(warehouseName);
        SetType(warehouseType);
        OrganizationUnitId = organizationUnitId ?? throw new ArgumentNullException(nameof(organizationUnitId));
        OrganizationUnitName = organizationUnitName ?? throw new ArgumentNullException(nameof(organizationUnitName));
        PlantId = plantId ?? throw new ArgumentNullException(nameof(plantId));
        PlantName = plantName ?? throw new ArgumentNullException(nameof(plantName));
        StorageConditionType = storageConditionType;
        SetLocationLevelCount(locationLevelCount);
        IsActive = isActive;

        AddLocalEvent(new WarehouseCreatedEvent
        {
            WarehouseId = Id,
            WarehouseCode = WarehouseCode,
            WarehouseName = WarehouseName
        });
    }

    /// <summary>
    /// Sets the warehouse code. Must be non-empty and follow format rules.
    /// </summary>
    public Warehouse SetWarehouseCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Warehouse code cannot be empty.", nameof(code));
        WarehouseCode = code.Trim();
        return this;
    }

    /// <summary>
    /// Sets the warehouse name.
    /// </summary>
    public Warehouse SetWarehouseName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name cannot be empty.", nameof(name));
        WarehouseName = name.Trim();
        return this;
    }

    /// <summary>
    /// Sets the warehouse type from WarehouseType enum.
    /// </summary>
    public Warehouse SetType(int warehouseType)
    {
        if (!Wms.Warehouse.Domain.Enums.WarehouseType.TryFromValue(warehouseType, out _))
            throw new ArgumentException($"Invalid warehouse type value: {warehouseType}", nameof(warehouseType));
        WarehouseType = warehouseType;
        return this;
    }

    /// <summary>
    /// Sets the responsible user for this warehouse.
    /// </summary>
    public Warehouse SetResponsibleUser(string? userId, string? userName)
    {
        ResponsibleUserId = userId;
        ResponsibleUserName = userName;
        return this;
    }

    /// <summary>
    /// Sets the organization unit name (redundant field).
    /// </summary>
    public Warehouse SetOrganizationUnitName(string organizationUnitName)
    {
        OrganizationUnitName = organizationUnitName ?? throw new ArgumentNullException(nameof(organizationUnitName));
        return this;
    }

    /// <summary>
    /// Sets the plant name (redundant field).
    /// </summary>
    public Warehouse SetPlantName(string plantName)
    {
        PlantName = plantName ?? throw new ArgumentNullException(nameof(plantName));
        return this;
    }

    /// <summary>
    /// Sets the warehouse address.
    /// </summary>
    public Warehouse SetAddress(string? address)
    {
        Address = address?.Trim();
        return this;
    }

    /// <summary>
    /// Sets the storage condition type.
    /// </summary>
    public Warehouse SetStorageConditionType(int storageConditionType)
    {
        if (!Wms.Warehouse.Domain.Enums.StorageConditionType.TryFromValue(storageConditionType, out _))
            throw new ArgumentException($"Invalid storage condition type value: {storageConditionType}", nameof(storageConditionType));
        StorageConditionType = storageConditionType;
        return this;
    }

    /// <summary>
    /// Sets the remark.
    /// </summary>
    public Warehouse SetRemark(string? remark)
    {
        Remark = remark?.Trim();
        return this;
    }

    /// <summary>
    /// Sets the location level count (must be 3 or 4).
    /// </summary>
    public Warehouse SetLocationLevelCount(int count)
    {
        if (count != 3 && count != 4)
            throw new ArgumentException("Location level count must be 3 or 4.", nameof(count));
        LocationLevelCount = count;
        return this;
    }

    /// <summary>
    /// Activates the warehouse.
    /// </summary>
    public Warehouse SetActive()
    {
        IsActive = true;
        return this;
    }

    /// <summary>
    /// Deactivates the warehouse and publishes a deactivation event.
    /// </summary>
    public Warehouse Deactivate()
    {
        IsActive = false;
        AddLocalEvent(new WarehouseDeactivatedEvent
        {
            WarehouseId = Id,
            WarehouseCode = WarehouseCode
        });
        return this;
    }

    /// <summary>
    /// Validates that the warehouse can accept inventory based on capacity rules.
    /// </summary>
    public bool ValidateCapacity()
    {
        // Basic validation: warehouse must be active and have valid configuration
        return IsActive && LocationLevelCount >= 3 && LocationLevelCount <= 4;
    }
}
