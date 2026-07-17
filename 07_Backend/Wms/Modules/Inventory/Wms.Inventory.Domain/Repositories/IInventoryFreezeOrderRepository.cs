using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Repositories;

/// <summary>
/// Inventory Freeze Order Repository Interface — specialized queries for freeze orders.
/// </summary>
public interface IInventoryFreezeOrderRepository : IRepository<InventoryFreezeOrder, Guid>
{
    /// <summary>Find by freeze order number (business natural key).</summary>
    Task<InventoryFreezeOrder?> FindByNoAsync(string freezeOrderNo);

    /// <summary>Get freeze orders by status.</summary>
    Task<List<InventoryFreezeOrder>> GetByStatusAsync(FreezeStatus status, int maxResultCount = 100, int skipCount = 0);

    /// <summary>Get freeze orders for a warehouse.</summary>
    Task<List<InventoryFreezeOrder>> GetByWarehouseAsync(Guid warehouseId, int maxResultCount = 100, int skipCount = 0);
}
