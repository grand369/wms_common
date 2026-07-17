using Volo.Abp.Domain.Repositories;
using Wms.Warehouse.Domain.Aggregates;

namespace Wms.Warehouse.Domain.Repositories;

/// <summary>
/// Location Repository Interface — extends ABP IRepository with custom query methods.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public interface ILocationRepository : IRepository<Location, Guid>
{
    /// <summary>
    /// Finds a location by its unique code.
    /// Returns null if not found.
    /// </summary>
    Task<Location?> FindByCodeAsync(string locationCode);

    /// <summary>
    /// Finds a location by its barcode ID (for PDA scanning).
    /// Returns null if not found.
    /// </summary>
    Task<Location?> FindByBarcodeIdAsync(string barcodeId);

    /// <summary>
    /// Gets all locations belonging to a specific warehouse.
    /// </summary>
    Task<List<Location>> GetListByWarehouseIdAsync(string warehouseId);

    /// <summary>
    /// Gets all locations belonging to a specific area.
    /// </summary>
    Task<List<Location>> GetListByAreaIdAsync(string areaId);

    /// <summary>
    /// Gets all available (active and with capacity) locations for putaway.
    /// </summary>
    Task<List<Location>> GetAvailableLocationsAsync(string warehouseId, int? storageCondition = null);

    /// <summary>
    /// Checks if a location code already exists (for uniqueness validation).
    /// </summary>
    Task<bool> CodeExistsAsync(string locationCode);
}
