using System;

namespace Wms.CycleCount.Application.Contracts.Dtos;

/// <summary>Command DTOs for cycle count business operations</summary>
public class SubmitCountCommandDto
{
    public Guid ItemId { get; set; }
    public decimal ActualQuantity { get; set; }
}
