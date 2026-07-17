using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inventory.HttpApi.Controllers;

/// <summary>
/// Inventory Ledger Controller — read-only API for ledger entry queries.
/// Route: /api/v1/inventory/ledger-entries
/// </summary>
[Route("api/v1/inventory/ledger-entries")]
[Authorize]
public class InventoryLedgerController : AbpControllerBase
{
    private readonly IInventoryLedgerAppService _appService;

    public InventoryLedgerController(IInventoryLedgerAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<InventoryLedgerOutputDto>> GetListAsync(InventoryLedgerQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<InventoryLedgerOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpGet("by-balance/{balanceId}")]
    public Task<List<InventoryLedgerOutputDto>> GetByBalanceIdAsync(Guid balanceId)
    {
        return _appService.GetByBalanceIdAsync(balanceId);
    }

    [HttpGet("by-source-order")]
    public Task<List<InventoryLedgerOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        return _appService.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);
    }

    [HttpGet("by-material-time/{materialId}")]
    public Task<List<InventoryLedgerOutputDto>> GetByMaterialTimeAsync(Guid materialId, DateTime? startTime = null, DateTime? endTime = null)
    {
        return _appService.GetByMaterialTimeAsync(materialId, startTime, endTime);
    }
}
