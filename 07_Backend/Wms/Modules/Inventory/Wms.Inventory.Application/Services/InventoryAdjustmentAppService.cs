using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Permissions;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Adjustment App Service — CRUD + state transition for adjustment records.
/// </summary>
public class InventoryAdjustmentAppService : ApplicationService, IInventoryAdjustmentAppService
{
    private readonly IInventoryAdjustmentRepository _adjustmentRepository;
    private readonly InventoryDomainService _domainService;

    public InventoryAdjustmentAppService(
        IInventoryAdjustmentRepository adjustmentRepository,
        InventoryDomainService domainService)
    {
        _adjustmentRepository = adjustmentRepository;
        _domainService = domainService;
    }

    [Authorize(WmsInventoryPermissions.Adjust.Create)]
    public async Task<InventoryAdjustmentOutputDto> CreateAsync(InventoryAdjustmentCreateDto dto)
    {
        var adjustment = new InventoryAdjustment(
            GuidGenerator.Create(),
            dto.AdjustmentNo,
            AdjustmentType.FromValue(dto.AdjustmentTypeValue),
            dto.AdjustmentReason,
            dto.WarehouseId,
            dto.WarehouseCode,
            dto.Remark);

        foreach (var lineDto in dto.Lines)
        {
            var line = new InventoryAdjustmentLine(
                GuidGenerator.Create(),
                adjustment.Id,
                lineDto.LineNo,
                lineDto.MaterialId,
                lineDto.MaterialCode,
                lineDto.MaterialName,
                lineDto.AdjustmentQuantity,
                lineDto.LocationId,
                lineDto.LocationCode,
                lineDto.BatchNumber,
                InventoryStatus.FromValue(lineDto.InventoryStatusBeforeValue),
                InventoryStatus.FromValue(lineDto.InventoryStatusAfterValue),
                lineDto.Reason);
            adjustment.AddLine(line);
        }

        await _adjustmentRepository.InsertAsync(adjustment);
        return MapToOutputDto(adjustment);
    }

    public async Task<InventoryAdjustmentOutputDto> GetAsync(Guid id)
    {
        var adjustment = await _adjustmentRepository.GetAsync(id);
        return MapToOutputDto(adjustment);
    }

    public async Task<PagedResultDto<InventoryAdjustmentOutputDto>> GetListAsync(InventoryAdjustmentQueryDto query)
    {
        var queryable = await _adjustmentRepository.GetQueryableAsync();
        if (query.WarehouseId.HasValue)
            queryable = queryable.Where(a => a.WarehouseId == query.WarehouseId.Value);
        if (query.ApprovalStatusValue.HasValue)
            queryable = queryable.Where(a => a.ApprovalStatus.Value == query.ApprovalStatusValue.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(a => a.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InventoryAdjustmentOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    [Authorize(WmsInventoryPermissions.Adjust.Submit)]
    public async Task<InventoryAdjustmentOutputDto> SubmitAsync(Guid id)
    {
        var adjustment = await _adjustmentRepository.GetAsync(id);
        adjustment.Submit();
        await _adjustmentRepository.UpdateAsync(adjustment);
        return MapToOutputDto(adjustment);
    }

    [Authorize(WmsInventoryPermissions.Adjust.Approve)]
    public async Task<InventoryAdjustmentOutputDto> ApproveAsync(Guid id)
    {
        var adjustment = await _adjustmentRepository.GetAsync(id);
        adjustment.Approve();
        await _adjustmentRepository.UpdateAsync(adjustment);
        return MapToOutputDto(adjustment);
    }

    [Authorize(WmsInventoryPermissions.Adjust.Approve)]
    public async Task<InventoryAdjustmentOutputDto> RejectAsync(Guid id)
    {
        var adjustment = await _adjustmentRepository.GetAsync(id);
        adjustment.Reject();
        await _adjustmentRepository.UpdateAsync(adjustment);
        return MapToOutputDto(adjustment);
    }

    [Authorize(WmsInventoryPermissions.Adjust.Execute)]
    public async Task<InventoryAdjustmentOutputDto> ExecuteAsync(Guid id)
    {
        var adjustment = await _adjustmentRepository.GetAsync(id);
        adjustment.Execute();
        await _domainService.AdjustInventoryAsync(id, adjustment);
        await _adjustmentRepository.UpdateAsync(adjustment);
        return MapToOutputDto(adjustment);
    }

    public async Task<InventoryAdjustmentOutputDto> CancelAsync(Guid id)
    {
        var adjustment = await _adjustmentRepository.GetAsync(id);
        adjustment.Cancel();
        await _adjustmentRepository.UpdateAsync(adjustment);
        return MapToOutputDto(adjustment);
    }

    private InventoryAdjustmentOutputDto MapToOutputDto(InventoryAdjustment adjustment)
    {
        return new InventoryAdjustmentOutputDto
        {
            Id = adjustment.Id,
            AdjustmentNo = adjustment.AdjustmentNo,
            AdjustmentTypeValue = adjustment.AdjustmentType.Value,
            AdjustmentTypeName = adjustment.AdjustmentType.Description,
            AdjustmentReason = adjustment.AdjustmentReason,
            ApprovalStatusValue = adjustment.ApprovalStatus.Value,
            ApprovalStatusName = adjustment.ApprovalStatus.Description,
            WarehouseId = adjustment.WarehouseId,
            WarehouseCode = adjustment.WarehouseCode,
            IsCompleted = adjustment.IsCompleted,
            CompletionTime = adjustment.CompletionTime,
            Remark = adjustment.Remark,
            Lines = adjustment.Lines.Select(l => new InventoryAdjustmentLineDto
            {
                LineNo = l.LineNo,
                MaterialId = l.MaterialId,
                MaterialCode = l.MaterialCode,
                MaterialName = l.MaterialName,
                AdjustmentQuantity = l.AdjustmentQuantity,
                LocationId = l.LocationId,
                LocationCode = l.LocationCode,
                BatchNumber = l.BatchNumber,
                InventoryStatusBeforeValue = l.InventoryStatusBefore.Value,
                InventoryStatusAfterValue = l.InventoryStatusAfter.Value,
                Reason = l.Reason
            }).ToList(),
            CreationTime = adjustment.CreationTime
        };
    }
}
