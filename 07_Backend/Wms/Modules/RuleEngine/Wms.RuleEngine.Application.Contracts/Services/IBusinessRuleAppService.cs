using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Wms.RuleEngine.Application.Contracts.Dtos;

namespace Wms.RuleEngine.Application.Contracts.Services;

/// <summary>
/// IBusinessRuleAppService — application service interface for business rule operations.
/// 7 methods matching API-RE-001~007.
/// </summary>
public interface IBusinessRuleAppService : IApplicationService
{
    /// <summary>Get paged list of business rules (API-RE-001).</summary>
    Task<PagedResultDto<BusinessRuleOutputDto>> GetListAsync(BusinessRuleQueryDto query);

    /// <summary>Get a business rule by ID (API-RE-002).</summary>
    Task<BusinessRuleOutputDto> GetAsync(Guid id);

    /// <summary>Create a business rule (API-RE-003).</summary>
    Task<BusinessRuleOutputDto> CreateAsync(BusinessRuleCreateDto dto);

    /// <summary>Update a business rule (API-RE-004).</summary>
    Task<BusinessRuleOutputDto> UpdateAsync(Guid id, BusinessRuleUpdateDto dto);

    /// <summary>Evaluate a business rule with context data (API-RE-005).</summary>
    Task<RuleEvaluateResultDto> EvaluateAsync(Guid id, RuleEvaluateDto dto);

    /// <summary>Get paged list of industry packages (API-RE-006).</summary>
    Task<PagedResultDto<IndustryPackageOutputDto>> GetPackageListAsync(IndustryPackageQueryDto query);

    /// <summary>Import an industry package (API-RE-007).</summary>
    Task<List<BusinessRuleOutputDto>> ImportPackageAsync(Guid packageId);
}
