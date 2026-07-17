using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Contracts.Services;

/// <summary>
/// Warehouse Area App Service Interface — defines CRUD + list by warehouse operations.
/// (API-WH-012~018, Phase 6 API Design)
/// </summary>
public interface IWarehouseAreaAppService
{
    Task<WarehouseAreaOutputDto> GetAsync(Guid id);
    Task<PagedResultDto<WarehouseAreaOutputDto>> GetListAsync(WarehouseAreaQueryDto query);
    Task<List<WarehouseAreaOutputDto>> GetListByWarehouseIdAsync(string warehouseId);
    Task<WarehouseAreaOutputDto> CreateAsync(WarehouseAreaCreateDto input);
    Task<WarehouseAreaOutputDto> UpdateAsync(Guid id, WarehouseAreaUpdateDto input);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}
