using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Services;

namespace Wms.Material.HttpApi.Controllers;

/// <summary>
/// Unit of Measure Controller — provides REST API for UoM CRUD + active list.
/// Route: /api/v1/material/units
/// (API-MT-020~024, Phase 6 API Design)
/// </summary>
[Route("api/v1/material/units")]
[Authorize]
public class UnitOfMeasureController : AbpControllerBase
{
    private readonly IUnitOfMeasureAppService _unitAppService;

    public UnitOfMeasureController(IUnitOfMeasureAppService unitAppService)
    {
        _unitAppService = unitAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<UnitOfMeasureOutputDto>> GetListAsync(UnitOfMeasureQueryDto query)
    {
        return _unitAppService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<UnitOfMeasureOutputDto> GetAsync(Guid id)
    {
        return _unitAppService.GetAsync(id);
    }

    [HttpGet("by-code/{unitCode}")]
    public Task<UnitOfMeasureOutputDto> GetByCodeAsync(string unitCode)
    {
        return _unitAppService.GetByCodeAsync(unitCode);
    }

    [HttpGet("active")]
    public Task<List<UnitOfMeasureOutputDto>> GetActiveListAsync()
    {
        return _unitAppService.GetActiveListAsync();
    }

    [HttpPost]
    public Task<UnitOfMeasureOutputDto> CreateAsync(UnitOfMeasureCreateDto input)
    {
        return _unitAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<UnitOfMeasureOutputDto> UpdateAsync(Guid id, UnitOfMeasureUpdateDto input)
    {
        return _unitAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _unitAppService.DeleteAsync(id);
    }
}
