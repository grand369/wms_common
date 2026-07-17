using Volo.Abp.Domain.Entities.Auditing;
using Wms.Material.Domain.Enums;

namespace Wms.Material.Domain.Entities;

/// <summary>
/// Unit of Measure Entity — represents a measurement unit.
/// Not an aggregate root but managed as an independent entity for CRUD operations.
/// Inherits FullAuditedEntity<Guid> for ABP audit fields and soft delete.
/// (ENT-04 reference, Phase 3 DDD Design)
/// </summary>
public class UnitOfMeasure : FullAuditedEntity<Guid>
{
    /// <summary>单位编码（唯一）</summary>
    public string UnitCode { get; private set; } = string.Empty;

    /// <summary>单位名称</summary>
    public string UnitName { get; private set; } = string.Empty;

    /// <summary>单位符号</summary>
    public string UnitSymbol { get; private set; } = string.Empty;

    /// <summary>单位类型枚举值</summary>
    public int UnitType { get; private set; }

    /// <summary>是否启用</summary>
    public bool IsActive { get; private set; } = true;

    public UnitOfMeasure() { }

    public UnitOfMeasure(
        Guid id,
        string unitCode,
        string unitName,
        string unitSymbol,
        int unitType,
        bool isActive = true)
    {
        Id = id;
        UnitCode = unitCode ?? throw new ArgumentNullException(nameof(unitCode));
        UnitName = unitName ?? throw new ArgumentNullException(nameof(unitName));
        UnitSymbol = unitSymbol ?? throw new ArgumentNullException(nameof(unitSymbol));
        UnitType = unitType;
        IsActive = isActive;
    }

    public UnitOfMeasure SetUnitCode(string code)
    {
        UnitCode = code ?? throw new ArgumentNullException(nameof(code));
        return this;
    }

    public UnitOfMeasure SetUnitName(string name)
    {
        UnitName = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>Sets the unit symbol.</summary>
    public UnitOfMeasure SetUnitSymbol(string symbol)
    {
        UnitSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        return this;
    }

    /// <summary>Sets the unit type.</summary>
    public UnitOfMeasure SetUnitType(int unitType)
    {
        if (!Enums.UnitType.TryFromValue(unitType, out _))
            throw new ArgumentException($"Invalid unit type value: {unitType}", nameof(unitType));
        UnitType = unitType;
        return this;
    }

    /// <summary>Activates this unit of measure.</summary>
    public UnitOfMeasure SetActive()
    {
        IsActive = true;
        return this;
    }

    /// <summary>Deactivates this unit of measure.</summary>
    public UnitOfMeasure Deactivate()
    {
        IsActive = false;
        return this;
    }
}
