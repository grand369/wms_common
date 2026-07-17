using System;
using System.Collections.Generic;

namespace Wms.LineSide.Application.Contracts.Dtos;

public class LineSideWarehouseOutputDto
{
    public Guid Id { get; set; }
    public string LineSideWarehouseCode { get; set; }
    public string LineSideWarehouseName { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }
    public Guid ProductionLineId { get; set; }
    public string ProductionLineName { get; set; }
    public Guid? WorkStationId { get; set; }
    public int ConsumptionModeValue { get; set; }
    public string ConsumptionModeDescription { get; set; }
    public List<LineSideKanbanItemOutputDto> KanbanItems { get; set; } = new();
}

public class LineSideKanbanItemOutputDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal MinQuantity { get; set; }
    public decimal MaxQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
}
