using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Warehouse.Application.Contracts.Dtos;
using Wms.Warehouse.Application.Contracts.Services;

namespace Wms.Warehouse.HttpApi.Controllers;

/// <summary>
/// Location Controller — provides REST API for Location CRUD + barcode lookup + available locations.
/// Route: /api/v1/warehouse/locations
/// (API-WH-019~028, Phase 6 API Design)
/// </summary>
[Route("api/v1/warehouse/locations")]
[Authorize]
public class LocationController : AbpControllerBase
{
    private readonly ILocationAppService _locationAppService;

    public LocationController(ILocationAppService locationAppService)
    {
        _locationAppService = locationAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<LocationOutputDto>> GetListAsync(LocationQueryDto query)
    {
        return _locationAppService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<LocationOutputDto> GetAsync(Guid id)
    {
        return _locationAppService.GetAsync(id);
    }

    [HttpGet("by-barcode/{barcodeId}")]
    public Task<LocationOutputDto> GetByBarcodeAsync(string barcodeId)
    {
        return _locationAppService.GetByBarcodeAsync(barcodeId);
    }

    [HttpGet("by-warehouse/{warehouseId}")]
    public Task<List<LocationOutputDto>> GetListByWarehouseIdAsync(string warehouseId)
    {
        return _locationAppService.GetListByWarehouseIdAsync(warehouseId);
    }

    [HttpGet("by-area/{areaId}")]
    public Task<List<LocationOutputDto>> GetListByAreaIdAsync(string areaId)
    {
        return _locationAppService.GetListByAreaIdAsync(areaId);
    }

    [HttpGet("available/{warehouseId}")]
    public Task<List<LocationOutputDto>> GetAvailableLocationsAsync(string warehouseId, [FromQuery] int? storageCondition = null)
    {
        return _locationAppService.GetAvailableLocationsAsync(warehouseId, storageCondition);
    }

    [HttpPost]
    public Task<LocationOutputDto> CreateAsync([FromBody] LocationCreateDto input)
    {
        return _locationAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<LocationOutputDto> UpdateAsync(Guid id, [FromBody] LocationUpdateDto input)
    {
        return _locationAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _locationAppService.DeleteAsync(id);
    }

    [HttpPatch("{id}/activate")]
    public Task ActivateAsync(Guid id)
    {
        return _locationAppService.ActivateAsync(id);
    }

    [HttpPatch("{id}/deactivate")]
    public Task DeactivateAsync(Guid id)
    {
        return _locationAppService.DeactivateAsync(id);
    }
}
