using System;
using System.Collections.Generic;

namespace Wms.CycleCount.Application.Contracts.Dtos;

public class CycleCountPlanOutputDto
{
    public Guid Id { get; set; }
    public string PlanNo { get; set; }
    public int CountMethodValue { get; set; }
    public string CountMethodDescription { get; set; }
    public int CountStatusValue { get; set; }
    public string CountStatusDescription { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public DateTime PlannedDate { get; set; }
    public bool FreezeInventory { get; set; }
    public decimal DifferenceThreshold { get; set; }
    public bool BlindCountEnabled { get; set; }
    public string? Remark { get; set; }
    public List<CycleCountItemOutputDto> Items { get; set; } = new();
}

public class CycleCountItemOutputDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public string? BatchNumber { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal? ActualQuantity { get; set; }
    public decimal DifferenceQuantity { get; set; }
}
