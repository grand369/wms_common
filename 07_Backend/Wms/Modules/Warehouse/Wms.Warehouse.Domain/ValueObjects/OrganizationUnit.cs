namespace Wms.Warehouse.Domain.ValueObjects;

/// <summary>
/// Organization Unit Value Object (VO-16) — represents the organizational hierarchy context.
/// Embedded as Owned Entity in Warehouse aggregate.
/// (ENT-01, Phase 3 DDD Design)
/// </summary>
public record OrganizationUnit
{
    public Guid UnitId { get; init; }
    public string UnitName { get; init; } = string.Empty;
    public string UnitType { get; init; } = string.Empty;
    public Guid? ParentUnitId { get; init; }

    public OrganizationUnit() { }

    public OrganizationUnit(Guid unitId, string unitName, string unitType = "", Guid? parentUnitId = null)
    {
        UnitId = unitId;
        UnitName = unitName ?? throw new ArgumentNullException(nameof(unitName));
        UnitType = unitType;
        ParentUnitId = parentUnitId;
    }
}
