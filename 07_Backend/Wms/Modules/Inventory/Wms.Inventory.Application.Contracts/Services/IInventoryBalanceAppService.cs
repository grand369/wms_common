using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Services;

/// <summary>
/// Inventory Balance App Service Interface — core service for querying and initializing inventory.
/// </summary>
public interface IInventoryBalanceAppService
{
    Task<PagedResultDto<InventoryBalanceOutputDto>> GetListAsync(InventoryBalanceQueryDto query);
    Task<InventoryBalanceOutputDto> GetAsync(Guid id);
    Task<List<InventoryBalanceOutputDto>> GetAvailableAsync(InventoryBalanceAvailableQueryDto query);
    Task<List<InventoryBalanceOutputDto>> GetByMaterialAsync(Guid materialId);
    Task<List<InventoryBalanceOutputDto>> GetByLocationAsync(Guid locationId);
    Task<List<InventoryBalanceOutputDto>> GetByWarehouseAsync(Guid warehouseId);
    Task<List<InventoryBalanceOutputDto>> GetByBatchAsync(string batchNumber);
    Task<InventorySummaryDto> GetSummaryAsync();
    Task<InventoryBalanceOutputDto> InitializeAsync(InventoryBalanceInitializeDto dto);
    Task SnapshotAsync();
}
