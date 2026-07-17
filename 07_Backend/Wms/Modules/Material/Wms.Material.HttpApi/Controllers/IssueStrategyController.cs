using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Services;

namespace Wms.Material.HttpApi.Controllers;

[Route("api/v1/material/issue-strategies")]
[Authorize]
public class IssueStrategyController : AbpControllerBase
{
    private readonly IIssueStrategyAppService _appService;

    public IssueStrategyController(IIssueStrategyAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<MaterialIssueStrategyOutputDto>> GetListAsync(MaterialIssueStrategyQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<MaterialIssueStrategyOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost]
    public Task<MaterialIssueStrategyOutputDto> CreateAsync([FromBody] MaterialIssueStrategyCreateDto input)
    {
        return _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<MaterialIssueStrategyOutputDto> UpdateAsync(Guid id, [FromBody] MaterialIssueStrategyUpdateDto input)
    {
        return _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }
}