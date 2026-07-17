using System;
using System.Collections.Generic;

namespace Wms.LineSide.Application.Contracts.Dtos;

public class LineSideWarehouseCreateDto
{
    public string LineSideWarehouseCode { get; set; }
    public string LineSideWarehouseName { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public Guid ProductionLineId { get; set; }
    public string ProductionLineName { get; set; }
    public Guid? WorkStationId { get; set; }
    public int ConsumptionModeValue { get; set; } = 1;
}

public class LineSideKanbanItemDto
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal MinQuantity { get; set; }
    public decimal MaxQuantity { get; set; }
}
