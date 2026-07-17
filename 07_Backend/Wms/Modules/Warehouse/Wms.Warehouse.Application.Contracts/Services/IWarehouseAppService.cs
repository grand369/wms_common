using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Contracts.Services;

/// <summary>
/// Warehouse App Service Interface — defines CRUD + business operations for Warehouse aggregate.
/// (API-WH-001~011, Phase 6 API Design)
/// </summary>
public interface IWarehouseAppService : IApplicationService
{
    Task<WarehouseOutputDto> GetAsync(Guid id);
    Task<WarehouseOutputDto> GetByCodeAsync(string warehouseCode);
    Task<PagedResultDto<WarehouseOutputDto>> GetListAsync(WarehouseQueryDto query);
    Task<List<WarehouseOutputDto>> GetAllListAsync();
    Task<WarehouseOutputDto> CreateAsync(WarehouseCreateDto input);
    Task<WarehouseOutputDto> UpdateAsync(Guid id, WarehouseUpdateDto input);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}
