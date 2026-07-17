using System;

namespace Wms.CycleCount.Application.Contracts.Dtos;

public class CycleCountPlanQueryDto
{
    public int? CountStatusValue { get; set; }
    public int? CountMethodValue { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? PlanNo { get; set; }
}
