using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inventory.HttpApi.Controllers;

/// <summary>
/// Inventory Balance Controller — core API for inventory queries and initialization.
/// Route: /api/v1/inventory/balances
/// </summary>
[Route("api/v1/inventory/balances")]
[Authorize]
public class InventoryBalanceController : AbpControllerBase
{
    private readonly IInventoryBalanceAppService _appService;

    public InventoryBalanceController(IInventoryBalanceAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<InventoryBalanceOutputDto>> GetListAsync(InventoryBalanceQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<InventoryBalanceOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpGet("available")]
    public Task<List<InventoryBalanceOutputDto>> GetAvailableAsync(InventoryBalanceAvailableQueryDto query)
    {
        return _appService.GetAvailableAsync(query);
    }

    [HttpGet("by-material/{materialId}")]
    public Task<List<InventoryBalanceOutputDto>> GetByMaterialAsync(Guid materialId)
    {
        return _appService.GetByMaterialAsync(materialId);
    }

    [HttpGet("by-location/{locationId}")]
    public Task<List<InventoryBalanceOutputDto>> GetByLocationAsync(Guid locationId)
    {
        return _appService.GetByLocationAsync(locationId);
    }

    [HttpGet("by-warehouse/{warehouseId}")]
    public Task<List<InventoryBalanceOutputDto>> GetByWarehouseAsync(Guid warehouseId)
    {
        return _appService.GetByWarehouseAsync(warehouseId);
    }

    [HttpGet("by-batch/{batchNumber}")]
    public Task<List<InventoryBalanceOutputDto>> GetByBatchAsync(string batchNumber)
    {
        return _appService.GetByBatchAsync(batchNumber);
    }

    [HttpGet("summary")]
    public Task<InventorySummaryDto> GetSummaryAsync()
    {
        return _appService.GetSummaryAsync();
    }

    [HttpPost("initialize")]
    public Task<InventoryBalanceOutputDto> InitializeAsync(InventoryBalanceInitializeDto dto)
    {
        return _appService.InitializeAsync(dto);
    }

    [HttpPost("snapshot")]
    public Task SnapshotAsync()
    {
        return _appService.SnapshotAsync();
    }

    [HttpPost("{id}/freeze")]
    public Task<InventoryBalanceOutputDto> FreezeAsync(Guid id, [FromBody] InventoryBalanceFreezeDto dto)
    {
        return _appService.FreezeAsync(id, dto);
    }

    [HttpPost("{id}/unfreeze")]
    public Task<InventoryBalanceOutputDto> UnfreezeAsync(Guid id)
    {
        return _appService.UnfreezeAsync(id);
    }
}
