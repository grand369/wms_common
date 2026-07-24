using Wms.Inbound.Application.Contracts.Dtos;
using Wms.Inbound.Application.Contracts.Permissions;
using Wms.Inbound.Application.Contracts.Services;
using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using Wms.Inbound.Domain.Repositories;
using Wms.Inbound.Domain.Services;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inbound.Application.Services;

/// <summary>
/// InboundOrderAppService — application service for inbound order operations.
/// Injects IInboundOrderRepository + InboundDomainService + IInventoryDomainService (cross-module).
/// ⚠️ Cross-module call: CompleteAsync synchronously calls IInventoryDomainService.IncreaseInventoryAsync
/// within the same UoW transaction (CROSS-002, Phase 6 API Design line 1670).
/// </summary>
public class InboundOrderAppService : ApplicationService, IInboundOrderAppService
{
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly InboundDomainService _inboundDomainService;
    private readonly IInventoryDomainService _inventoryDomainService;

    public InboundOrderAppService(
        IInboundOrderRepository inboundOrderRepository,
        InboundDomainService inboundDomainService,
        IInventoryDomainService inventoryDomainService)
    {
        _inboundOrderRepository = inboundOrderRepository;
        _inboundDomainService = inboundDomainService;
        _inventoryDomainService = inventoryDomainService;
    }

    [Authorize(WmsInboundPermissions.Order.Read)]
    public async Task<PagedResultDto<InboundOrderOutputDto>> GetListAsync(InboundOrderQueryDto query)
    {
        var queryable = await _inboundOrderRepository.GetQueryableAsync();

        if (query.InboundTypeValue.HasValue)
        {
            var type = InboundType.FromValue(query.InboundTypeValue.Value);
            queryable = queryable.Where(o => o.InboundType == type);
        }
        if (query.InboundStatusValue.HasValue)
        {
            var status = InboundStatus.FromValue(query.InboundStatusValue.Value);
            queryable = queryable.Where(o => o.InboundStatus == status);
        }
        if (query.WarehouseId.HasValue)
        {
            queryable = queryable.Where(o => o.WarehouseId == query.WarehouseId.Value);
        }
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(o =>
                o.InboundOrderNo.Contains(query.Keyword) ||
                o.WarehouseCode.Contains(query.Keyword) ||
                (o.SupplierName != null && o.SupplierName.Contains(query.Keyword)));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(o => o.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InboundOrderOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    [Authorize(WmsInboundPermissions.Order.Read)]
    public async Task<InboundOrderOutputDto> GetAsync(Guid id)
    {
        var order = await _inboundOrderRepository.GetWithLinesAsync(id);
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Create)]
    public async Task<InboundOrderOutputDto> CreateAsync(InboundOrderCreateDto dto)
    {
        var inboundType = InboundType.FromValue(dto.InboundTypeValue);

        var lineData = dto.Lines.Select(l =>
            (l.MaterialId, l.MaterialCode, l.MaterialName, l.Unit ?? string.Empty, l.PlanQuantity, 
             l.PutawayWarehouseId, l.PutawayWarehouseCode, l.PutawayAreaId, l.PutawayAreaCode, 
             l.PutawayLocationId, l.PutawayLocationCode, l.BatchNumber)).ToList();

        var order = await _inboundDomainService.CreateInboundOrderAsync(
            inboundType, dto.WarehouseId, dto.WarehouseCode,
            dto.OverReceiptRatio, dto.QualityInspectionRequired,
            dto.PurchaseOrderId, dto.PurchaseOrderNo,
            dto.ProductionOrderId, dto.ReturnOrderId,
            dto.SupplierId, dto.SupplierName,
            dto.Remark, lineData);

        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Update)]
    public async Task<InboundOrderOutputDto> UpdateAsync(Guid id, InboundOrderUpdateDto dto)
    {
        var order = await _inboundOrderRepository.GetWithLinesAsync(id);

        if (order.InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot update when order status is {order.InboundStatus.Name}. Only Draft allows update. (IN-004)");
        }

        // Update supplier
        order.SetSupplier(dto.SupplierId, dto.SupplierName);

        // Update purchase order
        order.SetPurchaseOrder(dto.PurchaseOrderId, dto.PurchaseOrderNo);

        // Update other fields
        order.SetRemark(dto.Remark);

        // Update lines if provided
        if (dto.Lines != null && dto.Lines.Count > 0)
        {
            var newLines = dto.Lines.Select((l, index) => new InboundLine(
                l.Id ?? GuidGenerator.Create(),
                id,
                index + 1,
                l.MaterialId,
                l.MaterialCode,
                l.MaterialName,
                l.Unit ?? string.Empty,
                l.PlanQuantity,
                l.BatchNumber,
                l.ExpiryDate,
                l.ProductionDate,
                l.Remark
            )).ToList();

            // Set putaway location for lines that have it
            for (int i = 0; i < newLines.Count; i++)
            {
                var line = newLines[i];
                var dtoLine = dto.Lines[i];
                if (dtoLine.PutawayLocationId.HasValue)
                {
                    line.SetPutawayLocation(
                        dtoLine.PutawayWarehouseId.Value,
                        dtoLine.PutawayWarehouseCode ?? "",
                        dtoLine.PutawayAreaId.Value,
                        dtoLine.PutawayAreaCode ?? "",
                        dtoLine.PutawayLocationId.Value,
                        dtoLine.PutawayLocationCode ?? "");
                }
            }

            order.UpdateLines(newLines);
        }

        await _inboundOrderRepository.UpdateAsync(order);
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var order = await _inboundOrderRepository.GetAsync(id);

        if (order.InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot delete when order status is {order.InboundStatus.Name}. Only Draft allows delete. (IN-004)");
        }

        await _inboundOrderRepository.DeleteAsync(id);
    }

    [Authorize(WmsInboundPermissions.Order.Confirm)]
    public async Task<InboundOrderOutputDto> ConfirmAsync(Guid id, InboundConfirmCommandDto dto)
    {
        var recvData = dto.Lines.Select(l => (l.LineId, l.ReceivedQuantity, l.BatchNumber)).ToList();
        var order = await _inboundDomainService.ConfirmReceiptAsync(id, recvData);
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.QualityInspect)]
    public async Task<InboundOrderOutputDto> QualityInspectAsync(Guid id, InboundQualityInspectCommandDto dto)
    {
        var order = await _inboundOrderRepository.GetAsync(id);

        // Start inspection if still in Confirmed
        if (order.InboundStatus == InboundStatus.Confirmed)
        {
            order.StartQualityInspection();
        }

        foreach (var lineDto in dto.Lines)
        {
            var qualityResult = QualityStatus.FromValue(lineDto.QualityResultValue);
            await _inboundDomainService.ProcessQualityInspectionAsync(id, lineDto.LineId, qualityResult);
        }

        order = await _inboundOrderRepository.GetAsync(id);
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Putaway)]
    public async Task<InboundOrderOutputDto> PutawayAsync(Guid id, InboundPutawayCommandDto dto)
    {
        var order = await _inboundOrderRepository.GetAsync(id);

        foreach (var lineDto in dto.Lines)
        {
            await _inboundDomainService.ConfirmPutawayAsync(
                id, lineDto.LineId, lineDto.PutawayWarehouseId, lineDto.PutawayWarehouseCode, 
                lineDto.PutawayAreaId, lineDto.PutawayAreaCode, lineDto.PutawayLocationId, lineDto.PutawayLocationCode, lineDto.Quantity);
        }

        order = await _inboundOrderRepository.GetAsync(id);
        return MapToOutputDto(order);
    }

    /// <summary>
    /// ⚠️ CRITICAL CROSS-MODULE CALL — CompleteAsync must synchronously call
    /// IInventoryDomainService.IncreaseInventoryAsync for each line within the same UoW.
    /// (CROSS-002, Phase 6 API Design line 1670)
    /// </summary>
    [Authorize(WmsInboundPermissions.Order.Complete)]
    public async Task<InboundOrderOutputDto> CompleteAsync(Guid id)
    {
        // Step 1: Complete the inbound order (state transition + event)
        var order = await _inboundDomainService.CompleteInboundOrderAsync(id);

        // Step 2: Synchronously increase inventory for each line (same UoW transaction)
        foreach (var line in order.Lines.Where(l => l.ReceivedQuantity > 0))
        {
            await _inventoryDomainService.IncreaseInventoryAsync(
                line.MaterialId,
                order.WarehouseId,
                line.PutawayLocationId ?? Guid.Empty,
                line.BatchNumber,
                line.ReceivedQuantity,
                line.MaterialCode,
                order.WarehouseCode,
                line.PutawayLocationCode ?? string.Empty,
                "InboundOrder",
                order.Id);
        }

        // Re-fetch to get latest state
        order = await _inboundOrderRepository.GetAsync(id);
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Cancel)]
    public async Task<InboundOrderOutputDto> CancelAsync(Guid id)
    {
        var order = await _inboundOrderRepository.GetAsync(id);
        order.Cancel();
        await _inboundOrderRepository.UpdateAsync(order);
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Read)]
    public async Task<List<InboundRecommendLocationResultDto>> RecommendPutawayLocationsAsync(Guid id, Guid lineId)
    {
        var results = await _inboundDomainService.RecommendPutawayLocationAsync(id, lineId);

        return results.Select(r => new InboundRecommendLocationResultDto
        {
            LocationId = r.LocationId,
            LocationCode = r.LocationCode,
            AvailableCapacity = r.AvailableCapacity,
            MaxCapacity = 0,
            Priority = r.Priority,
            ZoneName = null
        }).ToList();
    }

    [Authorize(WmsInboundPermissions.Order.Create)]
    public async Task<List<InboundOrderOutputDto>> BatchCreateAsync(List<InboundOrderCreateDto> dtos)
    {
        var results = new List<InboundOrderOutputDto>();
        foreach (var dto in dtos)
        {
            var result = await CreateAsync(dto);
            results.Add(result);
        }
        return results;
    }

    [Authorize(WmsInboundPermissions.Order.Read)]
    public async Task<InboundOrderOutputDto> GetByNoAsync(string orderNo)
    {
        var order = await _inboundOrderRepository.FindByNoAsync(orderNo);
        if (order == null)
        {
            throw new BusinessException("WMS:Inbound:OrderNotFound",
                $"Inbound order with number {orderNo} not found.");
        }
        return MapToOutputDto(order);
    }

    [Authorize(WmsInboundPermissions.Order.Read)]
    public async Task<InboundStatisticsDto> GetStatisticsAsync(InboundStatisticsQueryDto query)
    {
        var queryable = await _inboundOrderRepository.GetQueryableAsync();

        // Apply date filter if provided
        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(o => o.CreationTime >= query.StartDate.Value);
        }
        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(o => o.CreationTime <= query.EndDate.Value.AddDays(1));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        
        var pendingCount = await AsyncExecuter.CountAsync(queryable
            .Where(o => o.InboundStatus == InboundStatus.Draft || 
                        o.InboundStatus == InboundStatus.Confirmed ||
                        o.InboundStatus == InboundStatus.Inspecting ||
                        o.InboundStatus == InboundStatus.Putaway));

        var completedCount = await AsyncExecuter.CountAsync(queryable
            .Where(o => o.InboundStatus == InboundStatus.Completed));

        var today = DateTime.Today;
        var todayCount = await AsyncExecuter.CountAsync(queryable
            .Where(o => o.CreationTime >= today && o.CreationTime < today.AddDays(1)));

        return new InboundStatisticsDto
        {
            TotalCount = (int)totalCount,
            PendingCount = (int)pendingCount,
            CompletedCount = (int)completedCount,
            TodayCount = (int)todayCount
        };
    }

    private InboundOrderOutputDto MapToOutputDto(InboundOrder order)
    {
        return new InboundOrderOutputDto
        {
            Id = order.Id,
            InboundOrderNo = order.InboundOrderNo,
            InboundTypeValue = order.InboundType.Value,
            InboundTypeName = order.InboundType.Description,
            InboundStatusValue = order.InboundStatus.Value,
            InboundStatusName = order.InboundStatus.Description,
            WarehouseId = order.WarehouseId,
            WarehouseCode = order.WarehouseCode,
            PurchaseOrderId = order.PurchaseOrderId,
            PurchaseOrderNo = order.PurchaseOrderNo,
            ProductionOrderId = order.ProductionOrderId,
            ReturnOrderId = order.ReturnOrderId,
            SupplierId = order.SupplierId,
            SupplierName = order.SupplierName,
            OverReceiptRatio = order.OverReceiptRatio,
            QualityInspectionRequired = order.QualityInspectionRequired,
            TotalPlanQuantity = order.TotalPlanQuantity,
            TotalReceivedQuantity = order.TotalReceivedQuantity,
            IsCompleted = order.IsCompleted,
            CompletionTime = order.CompletionTime,
            ErpCallbackStatusValue = order.ErpCallbackStatus.Value,
            ErpCallbackStatusName = order.ErpCallbackStatus.Description,
            Remark = order.Remark,
            CreationTime = order.CreationTime,
            Lines = order.Lines.Select(MapLineToOutputDto).ToList()
        };
    }

    private InboundLineOutputDto MapLineToOutputDto(InboundLine line)
    {
        return new InboundLineOutputDto
        {
            Id = line.Id,
            InboundOrderId = line.InboundOrderId,
            LineNo = line.LineNo,
            MaterialId = line.MaterialId,
            MaterialCode = line.MaterialCode,
            MaterialName = line.MaterialName,
            Unit = line.Unit,
            PlanQuantity = line.PlanQuantity,
            ReceivedQuantity = line.ReceivedQuantity,
            BatchNumber = line.BatchNumber,
            SerialNumberList = line.SerialNumberList,
            QualityStatusValue = line.QualityStatus.Value,
            QualityStatusName = line.QualityStatus.Description,
            PutawayWarehouseId = line.PutawayWarehouseId,
            PutawayWarehouseCode = line.PutawayWarehouseCode,
            PutawayAreaId = line.PutawayAreaId,
            PutawayAreaCode = line.PutawayAreaCode,
            PutawayLocationId = line.PutawayLocationId,
            PutawayLocationCode = line.PutawayLocationCode,
            ExpiryDate = line.ExpiryDate,
            ProductionDate = line.ProductionDate,
            Remark = line.Remark
        };
    }
}
