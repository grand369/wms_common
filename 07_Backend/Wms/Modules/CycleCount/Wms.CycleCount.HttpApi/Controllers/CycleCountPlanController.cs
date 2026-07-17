using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.CycleCount.Application.Contracts.Dtos;
using Wms.CycleCount.Application.Contracts.Services;

namespace Wms.CycleCount.HttpApi.Controllers;

[RemoteService(Name = "WmsCycleCount")]
[Area("WmsCycleCount")]
[Route("api/v1/cycle-count/plans")]
[Authorize]
public class CycleCountPlanController : AbpControllerBase
{
    private readonly ICycleCountPlanAppService _appService;

    public CycleCountPlanController(ICycleCountPlanAppService appService) => _appService = appService;

    [HttpGet] public Task<PagedResultDto<CycleCountPlanOutputDto>> GetListAsync(CycleCountPlanQueryDto query) => _appService.GetListAsync(query);
    [HttpGet("{id}")] public Task<CycleCountPlanOutputDto> GetAsync(Guid id) => _appService.GetAsync(id);
    [HttpPost] public Task<CycleCountPlanOutputDto> CreateAsync(CycleCountPlanCreateDto input) => _appService.CreateAsync(input);
    [HttpPatch("{id}/start")] public Task<CycleCountPlanOutputDto> StartCountingAsync(Guid id) => _appService.StartCountingAsync(id);
    [HttpPatch("{id}/submit-count")] public Task<CycleCountPlanOutputDto> SubmitCountAsync(Guid id, [FromBody] List<SubmitCountCommandDto> items) => _appService.SubmitCountAsync(id, items);
    [HttpPatch("{id}/recount/{itemId}")] public Task<CycleCountPlanOutputDto> RecountAsync(Guid id, Guid itemId) => _appService.RecountAsync(id, itemId);
    [HttpPatch("{id}/confirm-difference")] public Task<CycleCountPlanOutputDto> ConfirmDifferenceAsync(Guid id) => _appService.ConfirmDifferenceAsync(id);
    [HttpPatch("{id}/generate-adjustment")] public Task<CycleCountPlanOutputDto> GenerateAdjustmentAsync(Guid id) => _appService.GenerateAdjustmentAsync(id);
    [HttpPatch("{id}/complete")] public Task<CycleCountPlanOutputDto> CompleteAsync(Guid id) => _appService.CompleteAsync(id);
}
