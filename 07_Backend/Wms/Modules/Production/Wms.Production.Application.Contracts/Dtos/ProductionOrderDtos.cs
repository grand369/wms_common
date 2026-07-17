using System;
using System.Collections.Generic;

namespace Wms.Production.Application.Contracts.Dtos;

public class ProductionOrderCreateDto
{
    public string ProductionOrderNo { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal PlanQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
}

public class ProductionOrderOutputDto
{
    public Guid Id { get; set; }
    public string ProductionOrderNo { get; set; }
    public int ProductionStatusValue { get; set; }
    public string ProductionStatusDescription { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal PlanQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
}

public class ProductionOrderQueryDto { public int? ProductionStatusValue { get; set; } public string? OrderNo { get; set; } }
