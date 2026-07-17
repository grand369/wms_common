using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Warehouse.Application.Contracts.Dtos;
using Wms.Warehouse.Application.Contracts.Services;

namespace Wms.Warehouse.HttpApi.Controllers;

/// <summary>
/// Warehouse Area Controller — provides REST API for WarehouseArea CRUD + status management.
/// Route: /api/v1/warehouse/areas
/// (API-WH-012~018, Phase 6 API Design)
/// </summary>
[Route("api/v1/warehouse/areas")]
[Authorize]
public class WarehouseAreaController : AbpControllerBase
{
    private readonly IWarehouseAreaAppService _areaAppService;

    public WarehouseAreaController(IWarehouseAreaAppService areaAppService)
    {
        _areaAppService = areaAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<WarehouseAreaOutputDto>> GetListAsync(WarehouseAreaQueryDto query)
    {
        return _areaAppService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<WarehouseAreaOutputDto> GetAsync(Guid id)
    {
        return _areaAppService.GetAsync(id);
    }

    [HttpGet("by-warehouse/{warehouseId}")]
    public Task<List<WarehouseAreaOutputDto>> GetListByWarehouseIdAsync(string warehouseId)
    {
        return _areaAppService.GetListByWarehouseIdAsync(warehouseId);
    }

    [HttpPost]
    public Task<WarehouseAreaOutputDto> CreateAsync([FromBody] WarehouseAreaCreateDto input)
    {
        return _areaAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<WarehouseAreaOutputDto> UpdateAsync(Guid id, [FromBody] WarehouseAreaUpdateDto input)
    {
        return _areaAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _areaAppService.DeleteAsync(id);
    }

    [HttpPatch("{id}/activate")]
    public Task ActivateAsync(Guid id)
    {
        return _areaAppService.ActivateAsync(id);
    }

    [HttpPatch("{id}/deactivate")]
    public Task DeactivateAsync(Guid id)
    {
        return _areaAppService.DeactivateAsync(id);
    }
}
