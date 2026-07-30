using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inventory.HttpApi.Controllers;

/// <summary>
/// Inventory Snapshot Controller — provides snapshot management APIs.
/// Route: /api/v1/inventory/snapshots
/// </summary>
[Route("api/v1/inventory/snapshots")]
[Authorize]
public class InventorySnapshotController : AbpControllerBase
{
    private readonly IInventorySnapshotAppService _appService;

    public InventorySnapshotController(IInventorySnapshotAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<InventorySnapshotOutputDto>> GetListAsync([FromQuery] InventorySnapshotQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<InventorySnapshotOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost]
    public Task<InventorySnapshotOutputDto> CreateAsync([FromBody] InventorySnapshotCreateDto dto)
    {
        return _appService.CreateAsync(dto);
    }
}
