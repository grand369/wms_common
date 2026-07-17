using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inventory.HttpApi.Controllers;

/// <summary>
/// Inventory Adjustment Controller — CRUD + state transition API.
/// Route: /api/v1/inventory/adjustments
/// </summary>
[Route("api/v1/inventory/adjustments")]
[Authorize]
public class InventoryAdjustmentController : AbpControllerBase
{
    private readonly IInventoryAdjustmentAppService _appService;

    public InventoryAdjustmentController(IInventoryAdjustmentAppService appService)
    {
        _appService = appService;
    }

    [HttpPost]
    public Task<InventoryAdjustmentOutputDto> CreateAsync(InventoryAdjustmentCreateDto dto)
    {
        return _appService.CreateAsync(dto);
    }

    [HttpGet("{id}")]
    public Task<InventoryAdjustmentOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<InventoryAdjustmentOutputDto>> GetListAsync(InventoryAdjustmentQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpPost("{id}/submit")]
    public Task<InventoryAdjustmentOutputDto> SubmitAsync(Guid id)
    {
        return _appService.SubmitAsync(id);
    }

    [HttpPost("{id}/approve")]
    public Task<InventoryAdjustmentOutputDto> ApproveAsync(Guid id)
    {
        return _appService.ApproveAsync(id);
    }

    [HttpPost("{id}/reject")]
    public Task<InventoryAdjustmentOutputDto> RejectAsync(Guid id)
    {
        return _appService.RejectAsync(id);
    }

    [HttpPost("{id}/execute")]
    public Task<InventoryAdjustmentOutputDto> ExecuteAsync(Guid id)
    {
        return _appService.ExecuteAsync(id);
    }

    [HttpPost("{id}/cancel")]
    public Task<InventoryAdjustmentOutputDto> CancelAsync(Guid id)
    {
        return _appService.CancelAsync(id);
    }
}
