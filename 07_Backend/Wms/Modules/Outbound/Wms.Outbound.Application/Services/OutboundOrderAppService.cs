using Wms.Outbound.Application.Contracts.Dtos;
using Wms.Outbound.Application.Contracts.Permissions;
using Wms.Outbound.Application.Contracts.Services;
using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;
using Wms.Outbound.Domain.Repositories;
using Wms.Outbound.Domain.Services;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Outbound.Application.Services;

/// <summary>
/// OutboundOrderAppService — application service for outbound order operations.
/// Injects IOutboundOrderRepository + OutboundDomainService + IInventoryDomainService (cross-module).
/// ⚠️ Cross-module calls:
/// - AllocateAsync → IInventoryDomainService.ReserveInventoryAsync (same UoW, CROSS-002)
/// - CompleteAsync → IInventoryDomainService.DecreaseInventoryAsync + ReleaseReservationAsync (same UoW)
/// - ReleaseAllocationAsync → IInventoryDomainService.ReleaseReservationAsync (same UoW)
/// </summary>
public class OutboundOrderAppService : ApplicationService, IOutboundOrderAppService
{
    private readonly IOutboundOrderRepository _outboundOrderRepository;
    private readonly OutboundDomainService _outboundDomainService;
    private readonly IInventoryDomainService _inventoryDomainService;

    public OutboundOrderAppService(
        IOutboundOrderRepository outboundOrderRepository,
        OutboundDomainService outboundDomainService,
        IInventoryDomainService inventoryDomainService)
    {
        _outboundOrderRepository = outboundOrderRepository;
        _outboundDomainService = outboundDomainService;
        _inventoryDomainService = inventoryDomainService;
    }

    [Authorize(WmsOutboundPermissions.Order.Read)]
    public async Task<PagedResultDto<OutboundOrderOutputDto>> GetListAsync(OutboundOrderQueryDto query)
    {
        var queryable = await _outboundOrderRepository.GetQueryableAsync();

        if (query.OutboundTypeValue.HasValue)
        {
            var type = OutboundType.FromValue(query.OutboundTypeValue.Value);
            queryable = queryable.Where(o => o.OutboundType == type);
        }
        if (query.OutboundStatusValue.HasValue)
        {
            var status = OutboundStatus.FromValue(query.OutboundStatusValue.Value);
            queryable = queryable.Where(o => o.OutboundStatus == status);
        }
        if (query.WarehouseId.HasValue)
        {
            queryable = queryable.Where(o => o.WarehouseId == query.WarehouseId.Value);
        }
        if (query.IsEmergency.HasValue)
        {
            queryable = queryable.Where(o => o.IsEmergency == query.IsEmergency.Value);
        }
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(o =>
                o.OutboundOrderNo.Contains(query.Keyword) ||
                o.WarehouseCode.Contains(query.Keyword));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(o => o.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<OutboundOrderOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    [Authorize(WmsOutboundPermissions.Order.Read)]
    public async Task<OutboundOrderOutputDto> GetAsync(Guid id)
    {
        var order = await _outboundOrderRepository.GetAsync(id);
        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Create)]
    public async Task<OutboundOrderOutputDto> CreateAsync(OutboundOrderCreateDto dto)
    {
        var outboundType = OutboundType.FromValue(dto.OutboundTypeValue);

        var lineData = dto.Lines.Select(l =>
            (l.MaterialId, l.MaterialCode, l.MaterialName, l.RequiredQuantity, l.IssueStrategyValue)).ToList();

        var order = await _outboundDomainService.CreateOutboundOrderAsync(
            outboundType, dto.WarehouseId, dto.WarehouseCode,
            dto.OverIssueRatio, dto.IsEmergency,
            dto.MaterialRequisitionId, dto.SalesOrderId, dto.ReturnMaterialOrderId,
            dto.Remark, lineData);

        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Update)]
    public async Task<OutboundOrderOutputDto> UpdateAsync(Guid id, OutboundOrderUpdateDto dto)
    {
        var order = await _outboundOrderRepository.GetAsync(id);

        if (order.OutboundStatus != OutboundStatus.Draft)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot update when order status is {order.OutboundStatus.Name}. Only Draft allows update. (OB-004)");
        }

        order.SetRemark(dto.Remark);
        await _outboundOrderRepository.UpdateAsync(order);
        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var order = await _outboundOrderRepository.GetAsync(id);

        if (order.OutboundStatus != OutboundStatus.Draft)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot delete when order status is {order.OutboundStatus.Name}. Only Draft allows delete. (OB-004)");
        }

        await _outboundOrderRepository.DeleteAsync(id);
    }

    /// <summary>
    /// ⚠️ CRITICAL CROSS-MODULE CALL — AllocateAsync synchronously calls
    /// IInventoryDomainService.ReserveInventoryAsync for each line within the same UoW.
    /// (CROSS-002, Phase 6 API Design)
    /// </summary>
    [Authorize(WmsOutboundPermissions.Order.Allocate)]
    public async Task<OutboundOrderOutputDto> AllocateAsync(Guid id, OutboundAllocateCommandDto dto)
    {
        var allocationData = dto.Lines.Select(l =>
            (l.LineId, l.AllocatedQuantity, l.LocationId ?? Guid.Empty, l.LocationCode ?? string.Empty)).ToList();

        var order = await _outboundDomainService.AllocateInventoryAsync(id, allocationData);
        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Picking)]
    public async Task<OutboundOrderOutputDto> PickingAsync(Guid id, OutboundPickingCommandDto dto)
    {
        var pickingData = dto.Lines.Select(l => (l.LineId, l.PickedQuantity)).ToList();
        var order = await _outboundDomainService.ConfirmPickingAsync(id, pickingData);
        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Shipping)]
    public async Task<OutboundOrderOutputDto> ShippingAsync(Guid id, OutboundShippingCommandDto dto)
    {
        var shippingData = dto.Lines.Select(l => (l.LineId, l.ShippedQuantity)).ToList();
        var order = await _outboundDomainService.ConfirmShippingAsync(id, shippingData);
        return MapToOutputDto(order);
    }

    /// <summary>
    /// ⚠️ CRITICAL CROSS-MODULE CALL — CompleteAsync synchronously calls:
    /// - IInventoryDomainService.DecreaseInventoryAsync (actual deduction)
    /// - IInventoryDomainService.ReleaseReservationAsync (release reservation)
    /// within the same UoW transaction (CROSS-002, Phase 6 API Design).
    /// </summary>
    [Authorize(WmsOutboundPermissions.Order.Complete)]
    public async Task<OutboundOrderOutputDto> CompleteAsync(Guid id)
    {
        // Step 1: Complete the outbound order (state transition + event)
        var order = await _outboundDomainService.CompleteOutboundOrderAsync(id);

        // Step 2: Synchronously decrease inventory + release reservation for each line (same UoW)
        foreach (var line in order.Lines.Where(l => l.ShippedQuantity > 0))
        {
            // Decrease actual inventory
            await _inventoryDomainService.DecreaseInventoryAsync(
                line.MaterialId,
                order.WarehouseId,
                line.PickingLocationId ?? Guid.Empty,
                line.BatchNumber,
                InventoryStatus.Available.Value,
                line.ShippedQuantity,
                "OutboundOrder",
                order.Id);

            // Release the reservation that was made during allocation
            await _inventoryDomainService.ReleaseReservationAsync(
                line.MaterialId,
                order.WarehouseId,
                line.PickingLocationId ?? Guid.Empty,
                line.BatchNumber,
                InventoryStatus.Available.Value,
                line.AllocatedQuantity,
                "OutboundOrder",
                order.Id);
        }

        // Re-fetch to get latest state
        order = await _outboundOrderRepository.GetAsync(id);
        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Cancel)]
    public async Task<OutboundOrderOutputDto> CancelAsync(Guid id)
    {
        var order = await _outboundOrderRepository.GetAsync(id);
        order.Cancel();
        await _outboundOrderRepository.UpdateAsync(order);
        return MapToOutputDto(order);
    }

    /// <summary>
    /// ⚠️ CRITICAL CROSS-MODULE CALL — ReleaseAllocationAsync synchronously calls
    /// IInventoryDomainService.ReleaseReservationAsync for each line within the same UoW.
    /// (CROSS-002)
    /// </summary>
    [Authorize(WmsOutboundPermissions.Order.ReleaseAllocation)]
    public async Task<OutboundOrderOutputDto> ReleaseAllocationAsync(Guid id)
    {
        var order = await _outboundOrderRepository.GetAsync(id);

        // Release reservation for each allocated line before transitioning state
        foreach (var line in order.Lines.Where(l => l.AllocatedQuantity > 0))
        {
            await _inventoryDomainService.ReleaseReservationAsync(
                line.MaterialId,
                order.WarehouseId,
                line.PickingLocationId ?? Guid.Empty,
                line.BatchNumber,
                InventoryStatus.Available.Value,
                line.AllocatedQuantity,
                "OutboundOrder",
                order.Id);
        }

        order.ReleaseAllocation();
        await _outboundOrderRepository.UpdateAsync(order);
        return MapToOutputDto(order);
    }

    [Authorize(WmsOutboundPermissions.Order.Read)]
    public async Task<OutboundOrderOutputDto> GetByNoAsync(string orderNo)
    {
        var order = await _outboundOrderRepository.FindByNoAsync(orderNo);
        if (order == null)
        {
            throw new BusinessException("WMS:Outbound:OrderNotFound",
                $"Outbound order with number {orderNo} not found.");
        }
        return MapToOutputDto(order);
    }

    private OutboundOrderOutputDto MapToOutputDto(OutboundOrder order)
    {
        return new OutboundOrderOutputDto
        {
            Id = order.Id,
            OutboundOrderNo = order.OutboundOrderNo,
            OutboundTypeValue = order.OutboundType.Value,
            OutboundTypeName = order.OutboundType.Description,
            OutboundStatusValue = order.OutboundStatus.Value,
            OutboundStatusName = order.OutboundStatus.Description,
            WarehouseId = order.WarehouseId,
            WarehouseCode = order.WarehouseCode,
            MaterialRequisitionId = order.MaterialRequisitionId,
            SalesOrderId = order.SalesOrderId,
            ReturnMaterialOrderId = order.ReturnMaterialOrderId,
            OverIssueRatio = order.OverIssueRatio,
            IsEmergency = order.IsEmergency,
            TotalRequiredQuantity = order.TotalRequiredQuantity,
            TotalAllocatedQuantity = order.TotalAllocatedQuantity,
            TotalPickedQuantity = order.TotalPickedQuantity,
            TotalShippedQuantity = order.TotalShippedQuantity,
            IsCompleted = order.IsCompleted,
            CompletionTime = order.CompletionTime,
            ErpCallbackStatusValue = order.ErpCallbackStatus.Value,
            ErpCallbackStatusName = order.ErpCallbackStatus.Description,
            Remark = order.Remark,
            CreationTime = order.CreationTime,
            Lines = order.Lines.Select(MapLineToOutputDto).ToList()
        };
    }

    private OutboundLineOutputDto MapLineToOutputDto(OutboundLine line)
    {
        return new OutboundLineOutputDto
        {
            Id = line.Id,
            OutboundOrderId = line.OutboundOrderId,
            LineNo = line.LineNo,
            MaterialId = line.MaterialId,
            MaterialCode = line.MaterialCode,
            MaterialName = line.MaterialName,
            RequiredQuantity = line.RequiredQuantity,
            AllocatedQuantity = line.AllocatedQuantity,
            PickedQuantity = line.PickedQuantity,
            ShippedQuantity = line.ShippedQuantity,
            PickingLocationId = line.PickingLocationId,
            PickingLocationCode = line.PickingLocationCode,
            IssueStrategyValue = line.IssueStrategy.Value,
            IssueStrategyName = line.IssueStrategy.Description,
            BatchNumber = line.BatchNumber,
            Remark = line.Remark
        };
    }
}
