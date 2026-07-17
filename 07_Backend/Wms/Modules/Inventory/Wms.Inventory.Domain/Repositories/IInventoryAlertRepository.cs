using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Repositories;

/// <summary>
/// Inventory Alert Repository Interface — specialized queries for alerts.
/// </summary>
public interface IInventoryAlertRepository : IRepository<InventoryAlert, Guid>
{
    /// <summary>Get all active (unresolved) alerts.</summary>
    Task<List<InventoryAlert>> GetActiveAlertsAsync(int maxResultCount = 100, int skipCount = 0);

    /// <summary>Get alerts by type.</summary>
    Task<List<InventoryAlert>> GetByTypeAsync(AlertType alertType, int maxResultCount = 100, int skipCount = 0);

    /// <summary>Get alerts for a specific material.</summary>
    Task<List<InventoryAlert>> GetByMaterialAsync(Guid materialId, int maxResultCount = 100, int skipCount = 0);
}
