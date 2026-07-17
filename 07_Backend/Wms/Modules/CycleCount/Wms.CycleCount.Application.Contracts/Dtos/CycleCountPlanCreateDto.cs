using System;
using System.Collections.Generic;

namespace Wms.CycleCount.Application.Contracts.Dtos;

public class CycleCountPlanCreateDto
{
    public string PlanNo { get; set; }
    public int CountMethodValue { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public DateTime PlannedDate { get; set; }
    public bool FreezeInventory { get; set; } = true;
    public decimal DifferenceThreshold { get; set; } = 2.0m;
    public bool BlindCountEnabled { get; set; } = true;
    public string? Remark { get; set; }
}

public class CycleCountItemDto
{
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public string? BatchNumber { get; set; }
}
