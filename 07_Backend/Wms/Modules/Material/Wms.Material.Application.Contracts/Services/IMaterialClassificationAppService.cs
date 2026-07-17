using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Services;

/// <summary>
/// Material Classification App Service Interface.
/// (API-MT-014~019, Phase 6 API Design)
/// </summary>
public interface IMaterialClassificationAppService
{
    Task<MaterialClassificationOutputDto> GetAsync(Guid id);
    Task<MaterialClassificationOutputDto> GetByCodeAsync(string classificationCode);
    Task<PagedResultDto<MaterialClassificationOutputDto>> GetListAsync(MaterialClassificationQueryDto query);
    Task<List<MaterialClassificationOutputDto>> GetTreeAsync();
    Task<MaterialClassificationOutputDto> CreateAsync(MaterialClassificationCreateDto input);
    Task<MaterialClassificationOutputDto> UpdateAsync(Guid id, MaterialClassificationUpdateDto input);
    Task DeleteAsync(Guid id);
}
