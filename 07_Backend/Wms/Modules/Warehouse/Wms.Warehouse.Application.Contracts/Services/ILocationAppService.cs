using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Contracts.Services;

/// <summary>
/// Location App Service Interface — defines CRUD + available locations + activate/deactivate operations.
/// (API-WH-019~028, Phase 6 API Design)
/// </summary>
public interface ILocationAppService
{
    Task<LocationOutputDto> GetAsync(Guid id);
    Task<PagedResultDto<LocationOutputDto>> GetListAsync(LocationQueryDto query);
    Task<List<LocationOutputDto>> GetListByWarehouseIdAsync(string warehouseId);
    Task<List<LocationOutputDto>> GetListByAreaIdAsync(string areaId);
    Task<List<LocationOutputDto>> GetAvailableLocationsAsync(string warehouseId, int? storageCondition = null);
    Task<LocationOutputDto> GetByBarcodeAsync(string barcodeId);
    Task<LocationOutputDto> CreateAsync(LocationCreateDto input);
    Task<LocationOutputDto> UpdateAsync(Guid id, LocationUpdateDto input);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}
