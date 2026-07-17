using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Services;

/// <summary>
/// Inventory Alert App Service Interface — query + resolve + scan operations.
/// </summary>
public interface IInventoryAlertAppService
{
    Task<PagedResultDto<InventoryAlertOutputDto>> GetListAsync(InventoryAlertQueryDto query);
    Task<List<InventoryAlertOutputDto>> GetActiveAsync();
    Task<InventoryAlertOutputDto> ResolveAsync(Guid id);
    Task ScanAsync();
}
