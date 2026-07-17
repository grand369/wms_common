using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inbound.Application.Contracts.Dtos;
using Wms.Inbound.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Inbound.HttpApi.Controllers;

/// <summary>
/// InboundOrder Controller — API endpoints for inbound order operations.
/// Route: /api/v1/inbound/orders (API-IN-001~013)
/// </summary>
[Route("api/v1/inbound/orders")]
[Authorize]
public class InboundOrderController : AbpControllerBase
{
    private readonly IInboundOrderAppService _appService;

    public InboundOrderController(IInboundOrderAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<InboundOrderOutputDto>> GetListAsync(InboundOrderQueryDto query)
    {
        return _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public Task<InboundOrderOutputDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost]
    public Task<InboundOrderOutputDto> CreateAsync(InboundOrderCreateDto dto)
    {
        return _appService.CreateAsync(dto);
    }

    [HttpPut("{id}")]
    public Task<InboundOrderOutputDto> UpdateAsync(Guid id, InboundOrderUpdateDto dto)
    {
        return _appService.UpdateAsync(id, dto);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }

    [HttpPatch("{id}/confirm")]
    public Task<InboundOrderOutputDto> ConfirmAsync(Guid id, InboundConfirmCommandDto dto)
    {
        return _appService.ConfirmAsync(id, dto);
    }

    [HttpPatch("{id}/quality-inspect")]
    public Task<InboundOrderOutputDto> QualityInspectAsync(Guid id, InboundQualityInspectCommandDto dto)
    {
        return _appService.QualityInspectAsync(id, dto);
    }

    [HttpPatch("{id}/putaway")]
    public Task<InboundOrderOutputDto> PutawayAsync(Guid id, InboundPutawayCommandDto dto)
    {
        return _appService.PutawayAsync(id, dto);
    }

    [HttpPatch("{id}/complete")]
    public Task<InboundOrderOutputDto> CompleteAsync(Guid id)
    {
        return _appService.CompleteAsync(id);
    }

    [HttpPatch("{id}/cancel")]
    public Task<InboundOrderOutputDto> CancelAsync(Guid id)
    {
        return _appService.CancelAsync(id);
    }

    [HttpGet("{id}/recommend-locations")]
    public Task<List<InboundRecommendLocationResultDto>> RecommendPutawayLocationsAsync(Guid id, Guid lineId)
    {
        return _appService.RecommendPutawayLocationsAsync(id, lineId);
    }

    [HttpPost("batch-create")]
    public Task<List<InboundOrderOutputDto>> BatchCreateAsync(List<InboundOrderCreateDto> dtos)
    {
        return _appService.BatchCreateAsync(dtos);
    }

    [HttpGet("by-no/{orderNo}")]
    public Task<InboundOrderOutputDto> GetByNoAsync(string orderNo)
    {
        return _appService.GetByNoAsync(orderNo);
    }
}
