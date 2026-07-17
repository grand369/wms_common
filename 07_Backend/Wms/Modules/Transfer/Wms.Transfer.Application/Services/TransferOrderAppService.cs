using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;
using Wms.Transfer.Application.Contracts.Dtos;
using Wms.Transfer.Application.Contracts.Permissions;
using Wms.Transfer.Application.Contracts.Services;
using Wms.Transfer.Domain.Aggregates;
using Wms.Transfer.Domain.Enums;
using Wms.Transfer.Domain.Repositories;
using Wms.Transfer.Domain.Services;

namespace Wms.Transfer.Application.Services;

/// <summary>
/// TransferOrderAppService — implements ITransferOrderAppService (10 methods)
/// Cross-module calls: IInventoryDomainService (decrease/increase), ITaskDomainService (create outbound/inbound tasks)
/// </summary>
[Authorize(WmsTransferPermissions.Read)]
public class TransferOrderAppService : ApplicationService, ITransferOrderAppService
{
    private readonly ITransferOrderRepository _repository;
    private readonly TransferDomainService _domainService;

    public TransferOrderAppService(
        ITransferOrderRepository repository,
        TransferDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    // ── CRUD ──────────────────────────────────────────────

    public async Task<PagedResultDto<TransferOrderOutputDto>> GetListAsync(TransferOrderQueryDto query)
    {
        var orders = await _repository.GetListAsync();
        // Apply filtering
        var filtered = orders.AsQueryable();
        if (query.TransferStatusValue.HasValue)
            filtered = filtered.Where(o => o.TransferStatus.Value == query.TransferStatusValue.Value);
        if (query.TransferTypeValue.HasValue)
            filtered = filtered.Where(o => o.TransferType.Value == query.TransferTypeValue.Value);
        if (query.SourceWarehouseId.HasValue)
            filtered = filtered.Where(o => o.SourceWarehouseId == query.SourceWarehouseId.Value);
        if (query.TargetWarehouseId.HasValue)
            filtered = filtered.Where(o => o.TargetWarehouseId == query.TargetWarehouseId.Value);
        if (!string.IsNullOrEmpty(query.TransferOrderNo))
            filtered = filtered.Where(o => o.TransferOrderNo.Contains(query.TransferOrderNo));

        var result = filtered.ToList();
        return new PagedResultDto<TransferOrderOutputDto>(
            result.Count,
            ObjectMapper.Map<List<TransferOrder>, List<TransferOrderOutputDto>>(result));
    }

    public async Task<TransferOrderOutputDto> GetAsync(Guid id)
    {
        var order = await _repository.GetWithLinesAsync(id);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Create)]
    public async Task<TransferOrderOutputDto> CreateAsync(TransferOrderCreateDto input)
    {
        var transferType = TransferType.FromValue(input.TransferTypeValue);
        var lines = input.Lines.Select(l => (l.MaterialId, l.MaterialCode, l.TransferQuantity)).ToList();

        var order = await _domainService.CreateTransferOrderAsync(
            input.TransferOrderNo,
            transferType,
            input.SourceWarehouseId,
            input.SourceWarehouseCode,
            input.TargetWarehouseId,
            input.TargetWarehouseCode,
            input.IsCrossCompany,
            lines,
            input.Remark);

        await _repository.InsertAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Update)]
    public async Task<TransferOrderOutputDto> UpdateAsync(Guid id, TransferOrderUpdateDto input)
    {
        var order = await _repository.GetAsync(id);
        // order.Remark cannot be set directly (DDD private setter); consider adding UpdateRemark domain method
        await _repository.UpdateAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    // ── Business Operations ───────────────────────────────

    [Authorize(WmsTransferPermissions.Submit)]
    public async Task<TransferOrderOutputDto> SubmitApprovalAsync(Guid id)
    {
        var order = await _repository.GetAsync(id);
        await _domainService.SubmitApprovalAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Approve)]
    public async Task<TransferOrderOutputDto> ApproveAsync(Guid id)
    {
        var order = await _repository.GetAsync(id);
        order.Approve();
        await _repository.UpdateAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Outbound)]
    public async Task<TransferOrderOutputDto> ConfirmOutboundAsync(Guid id, ConfirmTransferOutboundCommandDto input)
    {
        var order = await _repository.GetWithLinesAsync(id);
        // Update outbound confirmed quantities
        foreach (var lineDto in input.Lines)
            order.UpdateOutboundConfirmedQuantity(lineDto.LineNo, lineDto.ConfirmedQuantity);
        await _domainService.ConfirmTransferOutboundAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Inbound)]
    public async Task<TransferOrderOutputDto> ConfirmInboundAsync(Guid id, ConfirmTransferInboundCommandDto input)
    {
        var order = await _repository.GetWithLinesAsync(id);
        // Update inbound confirmed quantities
        foreach (var lineDto in input.Lines)
            order.UpdateInboundConfirmedQuantity(lineDto.LineNo, lineDto.ConfirmedQuantity);
        await _domainService.ConfirmTransferInboundAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }

    [Authorize(WmsTransferPermissions.Complete)]
    public async Task<TransferOrderOutputDto> CompleteAsync(Guid id)
    {
        var order = await _repository.GetAsync(id);
        order.Complete();
        await _repository.UpdateAsync(order);
        return ObjectMapper.Map<TransferOrder, TransferOrderOutputDto>(order);
    }
}
