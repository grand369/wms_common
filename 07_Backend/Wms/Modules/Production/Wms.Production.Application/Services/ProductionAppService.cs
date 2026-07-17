using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Wms.Production.Application.Contracts.Dtos;
using Wms.Production.Application.Contracts.Permissions;
using Wms.Production.Application.Contracts.Services;
using Wms.Production.Domain.Aggregates;
using Wms.Production.Domain.Enums;
using Wms.Production.Domain.Repositories;
using Wms.Production.Domain.Services;
using Wms.Shared.Domain.Interfaces;

namespace Wms.Production.Application.Services;

[Authorize(WmsProductionPermissions.Read)]
public class ProductionAppService : ApplicationService, IProductionAppService
{
    private readonly IMaterialRequisitionRepository _requisitionRepo;
    private readonly ProductionDomainService _domainService;
    private readonly IInventoryDomainService _inventoryDomainService;

    public ProductionAppService(
        IMaterialRequisitionRepository requisitionRepo,
        ProductionDomainService domainService,
        IInventoryDomainService inventoryDomainService)
    { _requisitionRepo = requisitionRepo; _domainService = domainService; _inventoryDomainService = inventoryDomainService; }

    // ── Production Order (simplified — stored as query from requisitions) ──
    public async Task<PagedResultDto<ProductionOrderOutputDto>> GetOrdersAsync(ProductionOrderQueryDto query)
    {
        var requisitions = await _requisitionRepo.GetListAsync();
        var filtered = requisitions.AsQueryable();
        if (query.ProductionStatusValue.HasValue) filtered = filtered.Where(r => r.RequisitionStatus.Value == query.ProductionStatusValue.Value);
        if (!string.IsNullOrEmpty(query.OrderNo)) filtered = filtered.Where(r => r.ProductionOrderNo.Contains(query.OrderNo));
        var result = filtered.ToList();
        // Map requisitions to production orders view
        var dtos = result.Select(r => new ProductionOrderOutputDto
        {
            Id = r.ProductionOrderId, ProductionOrderNo = r.ProductionOrderNo,
            WarehouseId = r.WarehouseId, WarehouseCode = r.WarehouseCode,
        }).ToList();
        return new PagedResultDto<ProductionOrderOutputDto>(dtos.Count, dtos);
    }

    public async Task<ProductionOrderOutputDto> GetOrderAsync(Guid id)
    {
        var requisitions = await _requisitionRepo.GetByProductionOrderAsync(id);
        var first = requisitions.FirstOrDefault() ?? throw new EntityNotFoundException(typeof(MaterialRequisition));
        return new ProductionOrderOutputDto { Id = first.ProductionOrderId, ProductionOrderNo = first.ProductionOrderNo, WarehouseId = first.WarehouseId, WarehouseCode = first.WarehouseCode };
    }

    [Authorize(WmsProductionPermissions.Create)]
    public async Task<ProductionOrderOutputDto> CreateOrderAsync(ProductionOrderCreateDto input)
    {
        // Production order is a conceptual entity in v1.0 — backed by MaterialRequisition
        return new ProductionOrderOutputDto { Id = Guid.NewGuid(), ProductionOrderNo = input.ProductionOrderNo, WarehouseId = input.WarehouseId, WarehouseCode = input.WarehouseCode, PlanQuantity = input.PlanQuantity };
    }

    // ── Material Requisition ──────────────────────────────
    public async Task<PagedResultDto<MaterialRequisitionOutputDto>> GetRequisitionsAsync(MaterialRequisitionQueryDto query)
    {
        var requisitions = await _requisitionRepo.GetListAsync();
        var filtered = requisitions.AsQueryable();
        if (query.ProductionOrderId.HasValue) filtered = filtered.Where(r => r.ProductionOrderId == query.ProductionOrderId.Value);
        if (!string.IsNullOrEmpty(query.RequisitionNo)) filtered = filtered.Where(r => r.RequisitionNo.Contains(query.RequisitionNo));
        var result = filtered.ToList();
        return new PagedResultDto<MaterialRequisitionOutputDto>(result.Count, ObjectMapper.Map<List<MaterialRequisition>, List<MaterialRequisitionOutputDto>>(result));
    }

    public async Task<MaterialRequisitionOutputDto> GetRequisitionAsync(Guid id)
    {
        var req = await _requisitionRepo.GetWithLinesAsync(id);
        return ObjectMapper.Map<MaterialRequisition, MaterialRequisitionOutputDto>(req);
    }

    [Authorize(WmsProductionPermissions.Create)]
    public async Task<MaterialRequisitionOutputDto> GenerateRequisitionFromOrderAsync(Guid orderId)
    {
        // TODO: Get BOM from Material module and expand
        var bomLines = new List<(Guid, string, decimal)> { (Guid.NewGuid(), "MAT-001", 100) };
        var requisition = await _domainService.GenerateRequisitionFromOrderAsync(
            $"MR-{orderId:N}", orderId, $"PO-{orderId:N}",
            Guid.NewGuid(), "WH-01", bomLines);
        await _requisitionRepo.InsertAsync(requisition);
        return ObjectMapper.Map<MaterialRequisition, MaterialRequisitionOutputDto>(requisition);
    }

    [Authorize(WmsProductionPermissions.Complete)]
    public async Task<ProductionOrderOutputDto> CompleteProductionAsync(Guid id)
    {
        // Cross-module: increase inventory for completed goods
        // TODO: Get product material info from production order
        return new ProductionOrderOutputDto { Id = id };
    }
}
