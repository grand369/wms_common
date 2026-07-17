using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.LineSide.Application.Contracts.Dtos;
using Wms.LineSide.Application.Contracts.Services;

namespace Wms.LineSide.HttpApi.Controllers;

[RemoteService(Name = "WmsLineSide")]
[Area("WmsLineSide")]
[Route("api/v1/line-side/warehouses")]
[Authorize]
public class LineSideWarehouseController : AbpControllerBase
{
    private readonly ILineSideWarehouseAppService _appService;
    public LineSideWarehouseController(ILineSideWarehouseAppService appService) => _appService = appService;

    [HttpGet] public Task<PagedResultDto<LineSideWarehouseOutputDto>> GetListAsync(LineSideWarehouseQueryDto query) => _appService.GetListAsync(query);
    [HttpGet("{id}")] public Task<LineSideWarehouseOutputDto> GetAsync(Guid id) => _appService.GetAsync(id);
    [HttpPost] public Task<LineSideWarehouseOutputDto> CreateAsync(LineSideWarehouseCreateDto input) => _appService.CreateAsync(input);
    [HttpPut("{id}")] public Task<LineSideWarehouseOutputDto> UpdateAsync(Guid id, LineSideWarehouseCreateDto input) => _appService.UpdateAsync(id, input);
    [HttpGet("{id}/kanban-items")] public Task<List<LineSideKanbanItemOutputDto>> GetKanbanItemsAsync(Guid id) => _appService.GetKanbanItemsAsync(id);
    [HttpPatch("{id}/trigger-replenishment")] public Task<LineSideWarehouseOutputDto> TriggerReplenishmentAsync(Guid id, [FromBody] TriggerReplenishmentCommandDto input) => _appService.TriggerReplenishmentAsync(id, input);
    [HttpPatch("{id}/backflush-consume")] public Task<LineSideWarehouseOutputDto> BackflushConsumeAsync(Guid id, [FromBody] BackflushConsumeCommandDto input) => _appService.BackflushConsumeAsync(id, input);
}
