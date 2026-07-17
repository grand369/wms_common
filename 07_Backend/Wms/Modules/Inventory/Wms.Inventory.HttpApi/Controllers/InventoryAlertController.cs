using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inventory.HttpApi.Controllers;

/// <summary>
/// Inventory Alert Controller — query + resolve + scan API.
/// Route: /api/v1/inventory/alerts
/// </summary>
[Route("api/v1/inventory/alerts")]
[Authorize]
public class InventoryAlertController : AbpControllerBase
{
    private readonly IInventoryAlertAppService _appService;

    public InventoryAlertController(IInventoryAlertAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<InventoryAlertOutputDto>> GetListAsync(InventoryAlertQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("active")]
    public Task<List<InventoryAlertOutputDto>> GetActiveAsync()
    {
        return _appService.GetActiveAsync();
    }

    [HttpPost("{id}/resolve")]
    public Task<InventoryAlertOutputDto> ResolveAsync(Guid id)
    {
        return _appService.ResolveAsync(id);
    }

    [HttpPost("scan")]
    public Task ScanAsync()
    {
        return _appService.ScanAsync();
    }
}
