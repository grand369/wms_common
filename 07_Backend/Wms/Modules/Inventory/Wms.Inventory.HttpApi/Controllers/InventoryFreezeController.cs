using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inventory.HttpApi.Controllers;

/// <summary>
/// Inventory Freeze Controller — CRUD + state transition API.
/// Route: /api/v1/inventory/freeze-orders
/// </summary>
[Route("api/v1/inventory/freeze-orders")]
[Authorize]
public class InventoryFreezeController : AbpControllerBase
{
    private readonly IInventoryFreezeAppService _appService;

    public InventoryFreezeController(IInventoryFreezeAppService appService)
    {
        _appService = appService;
    }

    [HttpPost]
    public Task<InventoryFreezeOutputDto> CreateAsync(InventoryFreezeCreateDto dto)
    {
        return _appService.CreateAsync(dto);
    }

    [HttpGet("{id}")]
    public Task<InventoryFreezeOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<InventoryFreezeOutputDto>> GetListAsync(InventoryFreezeQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpPost("{id}/approve")]
    public Task<InventoryFreezeOutputDto> ApproveAsync(Guid id)
    {
        return _appService.ApproveAsync(id);
    }

    [HttpPost("{id}/release")]
    public Task<InventoryFreezeOutputDto> ReleaseAsync(Guid id)
    {
        return _appService.ReleaseAsync(id);
    }

    [HttpPost("{id}/cancel")]
    public Task<InventoryFreezeOutputDto> CancelAsync(Guid id)
    {
        return _appService.CancelAsync(id);
    }
}
