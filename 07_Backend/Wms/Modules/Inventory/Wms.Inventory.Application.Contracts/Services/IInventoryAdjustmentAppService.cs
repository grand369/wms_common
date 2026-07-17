using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Services;

/// <summary>
/// Inventory Adjustment App Service Interface — CRUD + state transition operations.
/// </summary>
public interface IInventoryAdjustmentAppService
{
    Task<InventoryAdjustmentOutputDto> CreateAsync(InventoryAdjustmentCreateDto dto);
    Task<InventoryAdjustmentOutputDto> GetAsync(Guid id);
    Task<PagedResultDto<InventoryAdjustmentOutputDto>> GetListAsync(InventoryAdjustmentQueryDto query);
    Task<InventoryAdjustmentOutputDto> SubmitAsync(Guid id);
    Task<InventoryAdjustmentOutputDto> ApproveAsync(Guid id);
    Task<InventoryAdjustmentOutputDto> RejectAsync(Guid id);
    Task<InventoryAdjustmentOutputDto> ExecuteAsync(Guid id);
    Task<InventoryAdjustmentOutputDto> CancelAsync(Guid id);
}
