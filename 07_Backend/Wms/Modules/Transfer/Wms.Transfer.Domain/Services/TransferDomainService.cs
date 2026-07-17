using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;
using Wms.Transfer.Domain.Aggregates;
using Wms.Transfer.Domain.Enums;
using Wms.Transfer.Domain.Repositories;

namespace Wms.Transfer.Domain.Services;

/// <summary>
/// DS-06: TransferDomainService — domain logic for transfer operations.
/// Cross-module calls (within UoW): Inventory (decrease/increase), TaskCenter (create outbound/inbound tasks)
/// </summary>
public class TransferDomainService : DomainService
{
    private readonly ITransferOrderRepository _repository;
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly ITaskDomainService _taskDomainService;

    public TransferDomainService(
        ITransferOrderRepository repository,
        IInventoryDomainService inventoryDomainService,
        ITaskDomainService taskDomainService)
    {
        _repository = repository;
        _inventoryDomainService = inventoryDomainService;
        _taskDomainService = taskDomainService;
    }

    /// <summary>DS-06-01: Create a transfer order with validation</summary>
    public async Task<TransferOrder> CreateTransferOrderAsync(
        string orderNo,
        TransferType transferType,
        Guid sourceWarehouseId,
        string sourceWarehouseCode,
        Guid targetWarehouseId,
        string targetWarehouseCode,
        bool isCrossCompany,
        List<(Guid materialId, string materialCode, decimal qty)> lines,
        string? remark = null)
    {
        // BR-033: source and target cannot be the same
        if (sourceWarehouseId == targetWarehouseId)
            throw new BusinessException("Wms.Transfer:0401", "Source and target warehouse cannot be the same.");

        if (await _repository.FindByNoAsync(orderNo) != null)
            throw new BusinessException("Wms.Transfer:0402", $"Transfer order no '{orderNo}' already exists.");

        var order = new TransferOrder(
            GuidGenerator.Create(),
            orderNo,
            transferType,
            sourceWarehouseId,
            sourceWarehouseCode,
            targetWarehouseId,
            targetWarehouseCode,
            isCrossCompany,
            remark);

        foreach (var (materialId, materialCode, qty) in lines)
        {
            order.AddLine(materialId, materialCode, qty);
        }

        return order;
    }

    /// <summary>DS-06-02: Submit approval — validates status and triggers workflow</summary>
    public async Task SubmitApprovalAsync(TransferOrder order)
    {
        order.SubmitApproval();
        await _repository.UpdateAsync(order);
    }

    /// <summary>DS-06-03: Confirm outbound — decrease source inventory + create in-transit + create outbound task</summary>
    public async Task ConfirmTransferOutboundAsync(TransferOrder order)
    {
        order.ConfirmOutbound();

        // Cross-module: decrease source warehouse inventory for each line
        foreach (var line in order.Lines)
        {
            await _inventoryDomainService.DecreaseInventoryAsync(
                line.MaterialId,
                order.SourceWarehouseId,
                Guid.Empty,
                null,
                0,
                line.TransferQuantity,
                "Transfer",
                order.Id);
        }

        // Cross-module: create outbound pick task
        await _taskDomainService.CreateTaskFromOrderAsync(
            taskTypeValue: TaskType.Picking.Value,
            sourceOrderId: order.Id,
            sourceOrderType: "Transfer",
            sourceOrderNo: order.TransferOrderNo,
            warehouseId: order.SourceWarehouseId,
            warehouseCode: order.SourceWarehouseCode);

        await _repository.UpdateAsync(order);
    }

    /// <summary>DS-06-04: Confirm inbound — increase target inventory + clear in-transit + create inbound task</summary>
    public async Task ConfirmTransferInboundAsync(TransferOrder order)
    {
        order.ConfirmInbound();

        // Cross-module: increase target warehouse inventory for each line
        foreach (var line in order.Lines)
        {
            await _inventoryDomainService.IncreaseInventoryAsync(
                line.MaterialId,
                order.TargetWarehouseId,
                Guid.Empty,
                null,
                line.InboundConfirmedQuantity > 0 ? line.InboundConfirmedQuantity : line.TransferQuantity,
                line.MaterialCode,
                order.TargetWarehouseCode,
                "",
                "Transfer",
                order.Id);
        }

        // Cross-module: create inbound putaway task
        await _taskDomainService.CreateTaskFromOrderAsync(
            taskTypeValue: TaskType.Putaway.Value,
            sourceOrderId: order.Id,
            sourceOrderType: "Transfer",
            sourceOrderNo: order.TransferOrderNo,
            warehouseId: order.TargetWarehouseId,
            warehouseCode: order.TargetWarehouseCode);

        await _repository.UpdateAsync(order);
    }
}
