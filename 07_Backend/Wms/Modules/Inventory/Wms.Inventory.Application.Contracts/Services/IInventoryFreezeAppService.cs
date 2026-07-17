using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Services;

/// <summary>
/// Inventory Freeze App Service Interface — CRUD + state transition operations.
/// </summary>
public interface IInventoryFreezeAppService
{
    Task<InventoryFreezeOutputDto> CreateAsync(InventoryFreezeCreateDto dto);
    Task<InventoryFreezeOutputDto> GetAsync(Guid id);
    Task<PagedResultDto<InventoryFreezeOutputDto>> GetListAsync(InventoryFreezeQueryDto query);
    Task<InventoryFreezeOutputDto> ApproveAsync(Guid id);
    Task<InventoryFreezeOutputDto> ReleaseAsync(Guid id);
    Task<InventoryFreezeOutputDto> CancelAsync(Guid id);
}
