using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Services;

/// <summary>
/// Material App Service Interface — defines CRUD + business operations for Material aggregate.
/// (API-MT-001~013, Phase 6 API Design)
/// </summary>
public interface IMaterialAppService
{
    Task<MaterialOutputDto> GetAsync(Guid id);
    Task<MaterialOutputDto> GetByCodeAsync(string materialCode);
    Task<PagedResultDto<MaterialOutputDto>> GetListAsync(MaterialQueryDto query);
    Task<MaterialOutputDto> CreateAsync(MaterialCreateDto input);
    Task<MaterialOutputDto> UpdateAsync(Guid id, MaterialUpdateDto input);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
    Task<List<MaterialSubstituteRelationDto>> GetSubstitutesAsync(Guid materialId);
    Task<MaterialSubstituteRelationDto> AddSubstituteAsync(Guid materialId, Guid substituteMaterialId, string substituteMaterialCode, int priority = 1, decimal ratio = 1.0m);
    Task RemoveSubstituteAsync(Guid materialId, Guid substituteRelationId);
}
