using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Warehouse.Application.Contracts.Dtos;
using Wms.Warehouse.Application.Contracts.Services;

namespace Wms.Warehouse.HttpApi.Controllers;

/// <summary>
/// Warehouse Controller — provides REST API for Warehouse CRUD + status management.
/// Route: /api/v1/warehouse/warehouses
/// (API-WH-001~011, Phase 6 API Design)
/// </summary>
[Route("api/v1/warehouse/warehouses")]
[Authorize]
public class WarehouseController : AbpControllerBase
{
    private readonly IWarehouseAppService _warehouseAppService;

    public WarehouseController(IWarehouseAppService warehouseAppService)
    {
        _warehouseAppService = warehouseAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<WarehouseOutputDto>> GetListAsync(WarehouseQueryDto query)
    {
        return _warehouseAppService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<WarehouseOutputDto> GetAsync(Guid id)
    {
        return _warehouseAppService.GetAsync(id);
    }

    [HttpGet("by-code/{code}")]
    public Task<WarehouseOutputDto> GetByCodeAsync(string code)
    {
        return _warehouseAppService.GetByCodeAsync(code);
    }

    [HttpGet("all")]
    public Task<List<WarehouseOutputDto>> GetAllListAsync()
    {
        return _warehouseAppService.GetAllListAsync();
    }

    [HttpPost]
    public Task<WarehouseOutputDto> CreateAsync([FromBody] WarehouseCreateDto input)
    {
        return _warehouseAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<WarehouseOutputDto> UpdateAsync(Guid id, [FromBody] WarehouseUpdateDto input)
    {
        return _warehouseAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _warehouseAppService.DeleteAsync(id);
    }

    [HttpPatch("{id}/activate")]
    public Task ActivateAsync(Guid id)
    {
        return _warehouseAppService.ActivateAsync(id);
    }

    [HttpPatch("{id}/deactivate")]
    public Task DeactivateAsync(Guid id)
    {
        return _warehouseAppService.DeactivateAsync(id);
    }
}
