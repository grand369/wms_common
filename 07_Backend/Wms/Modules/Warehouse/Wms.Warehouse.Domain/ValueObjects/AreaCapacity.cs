namespace Wms.Warehouse.Domain.ValueObjects;

/// <summary>
/// Area Capacity Value Object — represents the capacity state of a warehouse area.
/// Contains MaxCapacity, CurrentCapacity, and percentage calculation method.
/// Embedded as Owned Entity in WarehouseArea aggregate.
/// (ENT-02, Phase 3 DDD Design)
/// </summary>
public record AreaCapacity
{
    public decimal? MaxCapacity { get; init; }
    public decimal? CurrentCapacity { get; init; }

    public AreaCapacity() { }

    public AreaCapacity(decimal? maxCapacity, decimal? currentCapacity = null)
    {
        MaxCapacity = maxCapacity;
        CurrentCapacity = currentCapacity;
    }

    /// <summary>
    /// Calculates the capacity utilization percentage.
    /// Returns null if MaxCapacity is null or zero.
    /// </summary>
    public decimal? UtilizationPercentage
    {
        get
        {
            if (MaxCapacity == null || MaxCapacity == 0 || CurrentCapacity == null)
                return null;
            return Math.Round(CurrentCapacity.Value / MaxCapacity.Value * 100, 2);
        }
    }

    /// <summary>
    /// Checks whether the area has available capacity for the specified additional quantity.
    /// </summary>
    public bool HasAvailableCapacity(decimal additionalQuantity)
    {
        if (MaxCapacity == null || CurrentCapacity == null)
            return true; // No capacity tracking configured
        return CurrentCapacity.Value + additionalQuantity <= MaxCapacity.Value;
    }
}
