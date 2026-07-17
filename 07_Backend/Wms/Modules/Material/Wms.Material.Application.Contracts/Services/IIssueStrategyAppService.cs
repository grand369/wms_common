using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Services;

public interface IIssueStrategyAppService
{
    Task<MaterialIssueStrategyOutputDto> GetAsync(Guid id);
    Task<PagedResultDto<MaterialIssueStrategyOutputDto>> GetListAsync(MaterialIssueStrategyQueryDto query);
    Task<MaterialIssueStrategyOutputDto> CreateAsync(MaterialIssueStrategyCreateDto input);
    Task<MaterialIssueStrategyOutputDto> UpdateAsync(Guid id, MaterialIssueStrategyUpdateDto input);
    Task DeleteAsync(Guid id);
}