using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Wms.LineSide.Application.Contracts.Dtos;
using Wms.LineSide.Application.Contracts.Permissions;
using Wms.LineSide.Application.Contracts.Services;
using Wms.LineSide.Domain.Aggregates;
using Wms.LineSide.Domain.Enums;
using Wms.LineSide.Domain.Repositories;
using Wms.LineSide.Domain.Services;

namespace Wms.LineSide.Application.Services;

[Authorize(WmsLineSidePermissions.Read)]
public class LineSideWarehouseAppService : ApplicationService, ILineSideWarehouseAppService
{
    private readonly ILineSideWarehouseRepository _repository;
    private readonly LineSideDomainService _domainService;

    public LineSideWarehouseAppService(ILineSideWarehouseRepository repository, LineSideDomainService domainService)
    { _repository = repository; _domainService = domainService; }

    public async Task<PagedResultDto<LineSideWarehouseOutputDto>> GetListAsync(LineSideWarehouseQueryDto query)
    {
        var list = await _repository.GetListAsync();
        var filtered = list.AsQueryable();
        if (query.ProductionLineId.HasValue) filtered = filtered.Where(w => w.ProductionLineId == query.ProductionLineId.Value);
        if (!string.IsNullOrEmpty(query.Code)) filtered = filtered.Where(w => w.LineSideWarehouseCode.Contains(query.Code));
        var result = filtered.ToList();
        return new PagedResultDto<LineSideWarehouseOutputDto>(result.Count, ObjectMapper.Map<List<LineSideWarehouse>, List<LineSideWarehouseOutputDto>>(result));
    }

    public async Task<LineSideWarehouseOutputDto> GetAsync(Guid id)
    {
        var lsw = await _repository.GetWithKanbanItemsAsync(id);
        return ObjectMapper.Map<LineSideWarehouse, LineSideWarehouseOutputDto>(lsw);
    }

    [Authorize(WmsLineSidePermissions.Create)]
    public async Task<LineSideWarehouseOutputDto> CreateAsync(LineSideWarehouseCreateDto input)
    {
        var lsw = await _domainService.CreateAsync(
            input.LineSideWarehouseCode, input.LineSideWarehouseName,
            input.WarehouseId, input.WarehouseCode,
            input.ProductionLineId, input.ProductionLineName,
            input.WorkStationId, ConsumptionMode.FromValue(input.ConsumptionModeValue));
        await _repository.InsertAsync(lsw);
        return ObjectMapper.Map<LineSideWarehouse, LineSideWarehouseOutputDto>(lsw);
    }

    [Authorize(WmsLineSidePermissions.Update)]
    public async Task<LineSideWarehouseOutputDto> UpdateAsync(Guid id, LineSideWarehouseCreateDto input)
    {
        var lsw = await _repository.GetAsync(id);
        await _repository.UpdateAsync(lsw);
        return ObjectMapper.Map<LineSideWarehouse, LineSideWarehouseOutputDto>(lsw);
    }

    public async Task<List<LineSideKanbanItemOutputDto>> GetKanbanItemsAsync(Guid id)
    {
        var lsw = await _repository.GetWithKanbanItemsAsync(id);
        return ObjectMapper.Map<List<LineSideKanbanItem>, List<LineSideKanbanItemOutputDto>>(lsw.KanbanItems);
    }

    [Authorize(WmsLineSidePermissions.Replenish)]
    public async Task<LineSideWarehouseOutputDto> TriggerReplenishmentAsync(Guid id, TriggerReplenishmentCommandDto input)
    {
        var lsw = await _repository.GetWithKanbanItemsAsync(id);
        await _domainService.TriggerReplenishmentAsync(lsw, input.MaterialId, input.ReplenishmentQuantity);
        return ObjectMapper.Map<LineSideWarehouse, LineSideWarehouseOutputDto>(lsw);
    }

    [Authorize(WmsLineSidePermissions.Backflush)]
    public async Task<LineSideWarehouseOutputDto> BackflushConsumeAsync(Guid id, BackflushConsumeCommandDto input)
    {
        var lsw = await _repository.GetWithKanbanItemsAsync(id);
        lsw.BackflushConsume(input.ProductionOrderId, input.MaterialId, input.ConsumeQuantity);
        await _repository.UpdateAsync(lsw);
        return ObjectMapper.Map<LineSideWarehouse, LineSideWarehouseOutputDto>(lsw);
    }
}
