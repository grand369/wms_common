using System;
using System.Collections.Generic;

namespace Wms.LineSide.Application.Contracts.Dtos;

public class LineSideWarehouseQueryDto
{
    public Guid? ProductionLineId { get; set; }
    public string? Code { get; set; }
}

public class TriggerReplenishmentCommandDto
{
    public Guid MaterialId { get; set; }
    public decimal ReplenishmentQuantity { get; set; }
}

public class BackflushConsumeCommandDto
{
    public Guid ProductionOrderId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal ConsumeQuantity { get; set; }
}
