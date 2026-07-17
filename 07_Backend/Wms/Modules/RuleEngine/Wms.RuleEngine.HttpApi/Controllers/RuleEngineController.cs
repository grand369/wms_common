using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Wms.RuleEngine.Application.Contracts.Dtos;
using Wms.RuleEngine.Application.Contracts.Services;

namespace Wms.RuleEngine.HttpApi.Controllers;

/// <summary>
/// RuleEngine Controller — REST API endpoints for business rules and industry packages.
/// Route: /api/v1/rule-engine
/// </summary>
[Route("api/v1/rule-engine")]
[Authorize]
public class RuleEngineController : AbpControllerBase
{
    private readonly IBusinessRuleAppService _appService;

    public RuleEngineController(IBusinessRuleAppService appService)
    {
        _appService = appService;
    }

    // -- Business Rules --

    [HttpGet("rules")]
    public Task<PagedResultDto<BusinessRuleOutputDto>> GetListAsync(BusinessRuleQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("rules/{id}")]
    public Task<BusinessRuleOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost("rules")]
    public Task<BusinessRuleOutputDto> CreateAsync(BusinessRuleCreateDto dto)
    {
        return _appService.CreateAsync(dto);
    }

    [HttpPut("rules/{id}")]
    public Task<BusinessRuleOutputDto> UpdateAsync(Guid id, BusinessRuleUpdateDto dto)
    {
        return _appService.UpdateAsync(id, dto);
    }

    [HttpPost("rules/{id}/evaluate")]
    public Task<RuleEvaluateResultDto> EvaluateAsync(Guid id, RuleEvaluateDto dto)
    {
        return _appService.EvaluateAsync(id, dto);
    }

    // -- Industry Packages --

    [HttpGet("packages")]
    public Task<PagedResultDto<IndustryPackageOutputDto>> GetPackageListAsync(IndustryPackageQueryDto query)
    {
        return _appService.GetPackageListAsync(query);
    }

    [HttpPost("packages/{id}/import")]
    public Task<List<BusinessRuleOutputDto>> ImportPackageAsync(Guid packageId)
    {
        return _appService.ImportPackageAsync(packageId);
    }
}
