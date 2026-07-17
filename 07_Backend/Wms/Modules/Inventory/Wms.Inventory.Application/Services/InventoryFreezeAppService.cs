using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Permissions;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Wms.Inventory.Domain.ValueObjects;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Freeze App Service — CRUD + state transitions for freeze orders.
/// </summary>
public class InventoryFreezeAppService : ApplicationService, IInventoryFreezeAppService
{
    private readonly IInventoryFreezeOrderRepository _freezeRepository;
    private readonly InventoryDomainService _domainService;

    public InventoryFreezeAppService(
        IInventoryFreezeOrderRepository freezeRepository,
        InventoryDomainService domainService)
    {
        _freezeRepository = freezeRepository;
        _domainService = domainService;
    }

    [Authorize(WmsInventoryPermissions.Freeze.Create)]
    public async Task<InventoryFreezeOutputDto> CreateAsync(InventoryFreezeCreateDto dto)
    {
        var freezeOrder = new InventoryFreezeOrder(
            GuidGenerator.Create(),
            dto.FreezeOrderNo,
            FreezeScope.FromValue(dto.FreezeScopeValue),
            dto.FreezeReason,
            dto.WarehouseId,
            dto.WarehouseCode,
            dto.FreezeStartTime,
            dto.FreezeEndTime,
            dto.Remark);

        await _freezeRepository.InsertAsync(freezeOrder);

        // Execute freeze on inventory
        var ranges = dto.FreezeRanges.Select(r => new FreezeRange
        {
            MaterialId = r.MaterialId,
            MaterialCode = r.MaterialCode,
            WarehouseId = r.WarehouseId,
            WarehouseCode = r.WarehouseCode,
            LocationId = r.LocationId,
            LocationCode = r.LocationCode,
            BatchNumber = r.BatchNumber
        }).ToList();

        await _domainService.FreezeInventoryAsync(freezeOrder.Id, freezeOrder.FreezeScope, ranges, freezeOrder.FreezeReason);

        return MapToOutputDto(freezeOrder);
    }

    public async Task<InventoryFreezeOutputDto> GetAsync(Guid id)
    {
        var freeze = await _freezeRepository.GetAsync(id);
        return MapToOutputDto(freeze);
    }

    public async Task<PagedResultDto<InventoryFreezeOutputDto>> GetListAsync(InventoryFreezeQueryDto query)
    {
        var queryable = await _freezeRepository.GetQueryableAsync();
        if (query.WarehouseId.HasValue)
            queryable = queryable.Where(f => f.WarehouseId == query.WarehouseId.Value);
        if (query.FreezeStatusValue.HasValue)
            queryable = queryable.Where(f => f.FreezeStatus.Value == query.FreezeStatusValue.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(f => f.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InventoryFreezeOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    [Authorize(WmsInventoryPermissions.Freeze.Approve)]
    public async Task<InventoryFreezeOutputDto> ApproveAsync(Guid id)
    {
        var freeze = await _freezeRepository.GetAsync(id);
        freeze.Approve();
        await _freezeRepository.UpdateAsync(freeze);
        return MapToOutputDto(freeze);
    }

    [Authorize(WmsInventoryPermissions.Freeze.Release)]
    public async Task<InventoryFreezeOutputDto> ReleaseAsync(Guid id)
    {
        var freeze = await _freezeRepository.GetAsync(id);
        freeze.Release();
        await _domainService.UnfreezeInventoryAsync(freeze.Id);
        await _freezeRepository.UpdateAsync(freeze);
        return MapToOutputDto(freeze);
    }

    [Authorize(WmsInventoryPermissions.Freeze.Cancel)]
    public async Task<InventoryFreezeOutputDto> CancelAsync(Guid id)
    {
        var freeze = await _freezeRepository.GetAsync(id);
        freeze.Cancel();
        await _freezeRepository.UpdateAsync(freeze);
        return MapToOutputDto(freeze);
    }

    private InventoryFreezeOutputDto MapToOutputDto(InventoryFreezeOrder freeze)
    {
        return new InventoryFreezeOutputDto
        {
            Id = freeze.Id,
            FreezeOrderNo = freeze.FreezeOrderNo,
            FreezeScopeValue = freeze.FreezeScope.Value,
            FreezeScopeName = freeze.FreezeScope.Description,
            FreezeReason = freeze.FreezeReason,
            FreezeStatusValue = freeze.FreezeStatus.Value,
            FreezeStatusName = freeze.FreezeStatus.Description,
            WarehouseId = freeze.WarehouseId,
            WarehouseCode = freeze.WarehouseCode,
            IsApproved = freeze.IsApproved,
            FreezeStartTime = freeze.FreezeStartTime,
            FreezeEndTime = freeze.FreezeEndTime,
            Remark = freeze.Remark,
            CreationTime = freeze.CreationTime
        };
    }
}
