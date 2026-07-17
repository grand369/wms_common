using System;
using System.Threading.Tasks;
using Wms.Production.Domain.Aggregates;
using Wms.Production.Domain.Enums;
using Wms.Production.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;

namespace Wms.Production.Domain.Services;

/// <summary>
/// DS-09: ProductionDomainService — domain logic for production operations.
/// Cross-module calls: Inventory (decrease for issue, increase for completion), Outbound (create pick order for issue)
/// </summary>
public class ProductionDomainService : DomainService
{
    private readonly IMaterialRequisitionRepository _requisitionRepository;

    public ProductionDomainService(IMaterialRequisitionRepository requisitionRepository)
    {
        _requisitionRepository = requisitionRepository;
    }

    /// <summary>Generate material requisition from production order (BOM expansion) — REQ-PD-001</summary>
    public async Task<MaterialRequisition> GenerateRequisitionFromOrderAsync(
        string requisitionNo, Guid productionOrderId, string productionOrderNo,
        Guid warehouseId, string warehouseCode,
        List<(Guid materialId, string materialCode, decimal requiredQty)> bomLines)
    {
        if (await _requisitionRepository.FindByNoAsync(requisitionNo) != null)
            throw new BusinessException("Wms.Production:0401", $"Requisition '{requisitionNo}' already exists for this order.");

        var requisition = new MaterialRequisition(
            GuidGenerator.Create(), requisitionNo, productionOrderId, productionOrderNo,
            warehouseId, warehouseCode);

        foreach (var (materialId, materialCode, requiredQty) in bomLines)
        {
            requisition.AddLine(bomLines.IndexOf((materialId, materialCode, requiredQty)) + 1, materialId, materialCode, requiredQty);
        }

        return requisition;
    }
}
