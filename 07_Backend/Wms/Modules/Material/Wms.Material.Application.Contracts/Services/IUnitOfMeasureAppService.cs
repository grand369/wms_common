using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Services;

/// <summary>
/// Unit of Measure App Service Interface.
/// (API-MT-020~024, Phase 6 API Design)
/// </summary>
public interface IUnitOfMeasureAppService
{
    Task<UnitOfMeasureOutputDto> GetAsync(Guid id);
    Task<UnitOfMeasureOutputDto> GetByCodeAsync(string unitCode);
    Task<PagedResultDto<UnitOfMeasureOutputDto>> GetListAsync(UnitOfMeasureQueryDto query);
    Task<List<UnitOfMeasureOutputDto>> GetActiveListAsync();
    Task<UnitOfMeasureOutputDto> CreateAsync(UnitOfMeasureCreateDto input);
    Task<UnitOfMeasureOutputDto> UpdateAsync(Guid id, UnitOfMeasureUpdateDto input);
    Task DeleteAsync(Guid id);
}
