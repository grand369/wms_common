using Volo.Abp.Application.Services;
using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Services;

/// <summary>
/// Inventory Snapshot Application Service Interface.
/// </summary>
public interface IInventorySnapshotAppService : IApplicationService
{
    Task<PagedResultDto<InventorySnapshotOutputDto>> GetListAsync(InventorySnapshotQueryDto query);
    Task<InventorySnapshotOutputDto> GetAsync(Guid id);
    Task<InventorySnapshotOutputDto> CreateAsync(InventorySnapshotCreateDto dto);
}
