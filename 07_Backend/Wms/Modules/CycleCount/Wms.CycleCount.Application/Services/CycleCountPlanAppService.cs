using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Wms.CycleCount.Application.Contracts.Dtos;
using Wms.CycleCount.Application.Contracts.Permissions;
using Wms.CycleCount.Application.Contracts.Services;
using Wms.CycleCount.Domain.Aggregates;
using Wms.CycleCount.Domain.Enums;
using Wms.CycleCount.Domain.Repositories;
using Wms.CycleCount.Domain.Services;

namespace Wms.CycleCount.Application.Services;

[Authorize(WmsCycleCountPermissions.Read)]
public class CycleCountPlanAppService : ApplicationService, ICycleCountPlanAppService
{
    private readonly ICycleCountPlanRepository _repository;
    private readonly CycleCountDomainService _domainService;

    public CycleCountPlanAppService(ICycleCountPlanRepository repository, CycleCountDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    public async Task<PagedResultDto<CycleCountPlanOutputDto>> GetListAsync(CycleCountPlanQueryDto query)
    {
        var plans = await _repository.GetListAsync();
        var filtered = plans.AsQueryable();
        if (query.CountStatusValue.HasValue) filtered = filtered.Where(p => p.CountStatus.Value == query.CountStatusValue.Value);
        if (query.CountMethodValue.HasValue) filtered = filtered.Where(p => p.CountMethod.Value == query.CountMethodValue.Value);
        if (query.WarehouseId.HasValue) filtered = filtered.Where(p => p.WarehouseId == query.WarehouseId.Value);
        if (!string.IsNullOrEmpty(query.PlanNo)) filtered = filtered.Where(p => p.PlanNo.Contains(query.PlanNo));
        var result = filtered.ToList();
        return new PagedResultDto<CycleCountPlanOutputDto>(result.Count, ObjectMapper.Map<List<CycleCountPlan>, List<CycleCountPlanOutputDto>>(result));
    }

    public async Task<CycleCountPlanOutputDto> GetAsync(Guid id)
    {
        var plan = await _repository.GetWithItemsAsync(id);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }

    [Authorize(WmsCycleCountPermissions.Create)]
    public async Task<CycleCountPlanOutputDto> CreateAsync(CycleCountPlanCreateDto input)
    {
        var plan = await _domainService.CreatePlanAsync(
            input.PlanNo, CountMethod.FromValue(input.CountMethodValue),
            input.WarehouseId, input.WarehouseCode, input.PlannedDate,
            input.FreezeInventory, input.DifferenceThreshold, input.BlindCountEnabled, input.Remark);
        await _repository.InsertAsync(plan);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }

    [Authorize(WmsCycleCountPermissions.Execute)]
    public async Task<CycleCountPlanOutputDto> StartCountingAsync(Guid id)
    {
        var plan = await _repository.GetAsync(id);
        await _domainService.StartCountingAsync(plan);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }

    [Authorize(WmsCycleCountPermissions.Execute)]
    public async Task<CycleCountPlanOutputDto> SubmitCountAsync(Guid id, List<SubmitCountCommandDto> items)
    {
        var plan = await _repository.GetWithItemsAsync(id);
        foreach (var item in items) plan.SubmitCountData(item.ItemId, item.ActualQuantity);
        plan.CheckDifferenceOverThreshold();
        await _repository.UpdateAsync(plan);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }

    [Authorize(WmsCycleCountPermissions.Execute)]
    public async Task<CycleCountPlanOutputDto> RecountAsync(Guid id, Guid itemId)
    {
        var plan = await _repository.GetWithItemsAsync(id);
        plan.RecountItem(itemId);
        await _repository.UpdateAsync(plan);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }

    [Authorize(WmsCycleCountPermissions.Confirm)]
    public async Task<CycleCountPlanOutputDto> ConfirmDifferenceAsync(Guid id)
    {
        var plan = await _repository.GetAsync(id);
        // TODO: Confirm differences after approval (if over threshold)
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }

    [Authorize(WmsCycleCountPermissions.Adjust)]
    public async Task<CycleCountPlanOutputDto> GenerateAdjustmentAsync(Guid id)
    {
        await _domainService.GenerateAdjustmentAsync(id);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(await _repository.GetAsync(id));
    }

    [Authorize(WmsCycleCountPermissions.Complete)]
    public async Task<CycleCountPlanOutputDto> CompleteAsync(Guid id)
    {
        var plan = await _repository.GetAsync(id);
        plan.CompleteCounting();
        await _repository.UpdateAsync(plan);
        return ObjectMapper.Map<CycleCountPlan, CycleCountPlanOutputDto>(plan);
    }
}
