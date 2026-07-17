using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Services;

namespace Wms.Material.HttpApi.Controllers;

/// <summary>
/// Material Controller — provides REST API for Material CRUD + substitute management + status management.
/// Route: /api/v1/material/materials
/// (API-MT-001~013, Phase 6 API Design)
/// </summary>
[Route("api/v1/material/materials")]
[Authorize]
public class MaterialController : AbpControllerBase
{
    private readonly IMaterialAppService _materialAppService;

    public MaterialController(IMaterialAppService materialAppService)
    {
        _materialAppService = materialAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<MaterialOutputDto>> GetListAsync(MaterialQueryDto query)
    {
        return _materialAppService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<MaterialOutputDto> GetAsync(Guid id)
    {
        return _materialAppService.GetAsync(id);
    }

    [HttpGet("by-code/{materialCode}")]
    public Task<MaterialOutputDto> GetByCodeAsync(string materialCode)
    {
        return _materialAppService.GetByCodeAsync(materialCode);
    }

    [HttpPost]
    public Task<MaterialOutputDto> CreateAsync([FromBody] MaterialCreateDto input)
    {
        return _materialAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<MaterialOutputDto> UpdateAsync(Guid id, [FromBody] MaterialUpdateDto input)
    {
        return _materialAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _materialAppService.DeleteAsync(id);
    }

    [HttpPatch("{id}/activate")]
    public Task ActivateAsync(Guid id)
    {
        return _materialAppService.ActivateAsync(id);
    }

    [HttpPatch("{id}/deactivate")]
    public Task DeactivateAsync(Guid id)
    {
        return _materialAppService.DeactivateAsync(id);
    }

    [HttpGet("{materialId}/substitutes")]
    public Task<List<MaterialSubstituteRelationDto>> GetSubstitutesAsync(Guid materialId)
    {
        return _materialAppService.GetSubstitutesAsync(materialId);
    }

    [HttpPost("{materialId}/substitutes")]
    public Task<MaterialSubstituteRelationDto> AddSubstituteAsync(Guid materialId, [FromBody] AddSubstituteRequest request)
    {
        return _materialAppService.AddSubstituteAsync(materialId, request.SubstituteMaterialId, request.SubstituteMaterialCode, request.Priority, request.Ratio);
    }

    [HttpDelete("{materialId}/substitutes/{substituteRelationId}")]
    public Task RemoveSubstituteAsync(Guid materialId, Guid substituteRelationId)
    {
        return _materialAppService.RemoveSubstituteAsync(materialId, substituteRelationId);
    }
}

/// <summary>
/// Request body for adding a substitute material relation.
/// </summary>
public class AddSubstituteRequest
{
    public Guid SubstituteMaterialId { get; set; }
    public string SubstituteMaterialCode { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public decimal Ratio { get; set; } = 1.0m;
}
