using System;
using System.Collections.Generic;

namespace Wms.Production.Application.Contracts.Dtos;

public class MaterialRequisitionOutputDto
{
    public Guid Id { get; set; }
    public string RequisitionNo { get; set; }
    public Guid ProductionOrderId { get; set; }
    public string ProductionOrderNo { get; set; }
    public int RequisitionStatusValue { get; set; }
    public string RequisitionStatusDescription { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public List<MaterialRequisitionLineOutputDto> Lines { get; set; } = new();
}

public class MaterialRequisitionLineOutputDto
{
    public Guid Id { get; set; }
    public int LineNo { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
}

public class MaterialRequisitionQueryDto { public Guid? ProductionOrderId { get; set; } public string? RequisitionNo { get; set; }
}
