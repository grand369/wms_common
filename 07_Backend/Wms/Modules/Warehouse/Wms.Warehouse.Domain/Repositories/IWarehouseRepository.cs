using Volo.Abp.Domain.Repositories;
using WarehouseAgg = Wms.Warehouse.Domain.Aggregates.Warehouse;

namespace Wms.Warehouse.Domain.Repositories;

/// <summary>
/// Warehouse Repository Interface — extends ABP IRepository with custom query methods.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public interface IWarehouseRepository : IRepository<WarehouseAgg, Guid>
{
    /// <summary>
    /// Finds a warehouse by its unique code.
    /// Returns null if not found.
    /// </summary>
    Task<WarehouseAgg?> FindByCodeAsync(string warehouseCode);

    /// <summary>
    /// Gets all warehouses belonging to a specific organization unit.
    /// </summary>
    Task<List<WarehouseAgg>> GetListByOrganizationIdAsync(string organizationUnitId);

    /// <summary>
    /// Gets all active warehouses.
    /// </summary>
    Task<List<WarehouseAgg>> GetActiveListAsync();

    /// <summary>
    /// Checks if a warehouse code already exists (for uniqueness validation).
    /// </summary>
    Task<bool> CodeExistsAsync(string warehouseCode);
}
