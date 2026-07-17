using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Outbound.Application.Contracts.Dtos;
using Wms.Outbound.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Outbound.HttpApi.Controllers;

/// <summary>
/// OutboundOrder Controller — API endpoints for outbound order operations.
/// Route: /api/v1/outbound/orders (API-OB-001~012)
/// </summary>
[Route("api/v1/outbound/orders")]
[Authorize]
public class OutboundOrderController : AbpControllerBase
{
    private readonly IOutboundOrderAppService _appService;

    public OutboundOrderController(IOutboundOrderAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<OutboundOrderOutputDto>> GetListAsync(OutboundOrderQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<OutboundOrderOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost]
    public Task<OutboundOrderOutputDto> CreateAsync(OutboundOrderCreateDto dto)
    {
        return _appService.CreateAsync(dto);
    }

    [HttpPut("{id}")]
    public Task<OutboundOrderOutputDto> UpdateAsync(Guid id, OutboundOrderUpdateDto dto)
    {
        return _appService.UpdateAsync(id, dto);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }

    [HttpPatch("{id}/allocate")]
    public Task<OutboundOrderOutputDto> AllocateAsync(Guid id, OutboundAllocateCommandDto dto)
    {
        return _appService.AllocateAsync(id, dto);
    }

    [HttpPatch("{id}/picking")]
    public Task<OutboundOrderOutputDto> PickingAsync(Guid id, OutboundPickingCommandDto dto)
    {
        return _appService.PickingAsync(id, dto);
    }

    [HttpPatch("{id}/shipping")]
    public Task<OutboundOrderOutputDto> ShippingAsync(Guid id, OutboundShippingCommandDto dto)
    {
        return _appService.ShippingAsync(id, dto);
    }

    [HttpPatch("{id}/complete")]
    public Task<OutboundOrderOutputDto> CompleteAsync(Guid id)
    {
        return _appService.CompleteAsync(id);
    }

    [HttpPatch("{id}/cancel")]
    public Task<OutboundOrderOutputDto> CancelAsync(Guid id)
    {
        return _appService.CancelAsync(id);
    }

    [HttpPatch("{id}/release-allocation")]
    public Task<OutboundOrderOutputDto> ReleaseAllocationAsync(Guid id)
    {
        return _appService.ReleaseAllocationAsync(id);
    }

    [HttpGet("by-no/{orderNo}")]
    public Task<OutboundOrderOutputDto> GetByNoAsync(string orderNo)
    {
        return _appService.GetByNoAsync(orderNo);
    }
}
