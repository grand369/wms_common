using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Services;

namespace Wms.Material.HttpApi.Controllers;

/// <summary>
/// Material Classification Controller — provides REST API for classification CRUD + tree structure.
/// Route: /api/v1/material/classifications
/// (API-MT-014~019, Phase 6 API Design)
/// </summary>
[Route("api/v1/material/classifications")]
[Authorize]
public class MaterialClassificationController : AbpControllerBase
{
    private readonly IMaterialClassificationAppService _classificationAppService;

    public MaterialClassificationController(IMaterialClassificationAppService classificationAppService)
    {
        _classificationAppService = classificationAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<MaterialClassificationOutputDto>> GetListAsync(MaterialClassificationQueryDto query)
    {
        return _classificationAppService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<MaterialClassificationOutputDto> GetAsync(Guid id)
    {
        return _classificationAppService.GetAsync(id);
    }

    [HttpGet("by-code/{classificationCode}")]
    public Task<MaterialClassificationOutputDto> GetByCodeAsync(string classificationCode)
    {
        return _classificationAppService.GetByCodeAsync(classificationCode);
    }

    [HttpGet("tree")]
    public Task<List<MaterialClassificationOutputDto>> GetTreeAsync()
    {
        return _classificationAppService.GetTreeAsync();
    }

    [HttpPost]
    public Task<MaterialClassificationOutputDto> CreateAsync([FromBody]MaterialClassificationCreateDto input)
    {
        return _classificationAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<MaterialClassificationOutputDto> UpdateAsync(Guid id, [FromBody] MaterialClassificationUpdateDto input)
    {
        return _classificationAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _classificationAppService.DeleteAsync(id);
    }
}
