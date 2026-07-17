using Volo.Abp.Domain.Repositories;
using Wms.Warehouse.Domain.Aggregates;

namespace Wms.Warehouse.Domain.Repositories;

/// <summary>
/// Warehouse Area Repository Interface — extends ABP IRepository with custom query methods.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public interface IWarehouseAreaRepository : IRepository<WarehouseArea, Guid>
{
    /// <summary>
    /// Finds an area by its code within a specific warehouse.
    /// Returns null if not found.
    /// </summary>
    Task<WarehouseArea?> FindByCodeAndWarehouseIdAsync(string areaCode, string warehouseId);

    /// <summary>
    /// Gets all areas belonging to a specific warehouse.
    /// </summary>
    Task<List<WarehouseArea>> GetListByWarehouseIdAsync(string warehouseId);

    /// <summary>
    /// Checks if an area code already exists within a warehouse.
    /// </summary>
    Task<bool> CodeExistsInWarehouseAsync(string areaCode, string warehouseId);
}
