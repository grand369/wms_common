using System;
using System.Collections.Generic;

namespace Wms.Transfer.Application.Contracts.Dtos;

/// <summary>TransferOrder Create DTO — used for API-TF-003</summary>
public class TransferOrderCreateDto
{
    public string TransferOrderNo { get; set; }
    public int TransferTypeValue { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public string SourceWarehouseCode { get; set; }
    public Guid TargetWarehouseId { get; set; }
    public string TargetWarehouseCode { get; set; }
    public bool IsCrossCompany { get; set; }
    public string? Remark { get; set; }
    public List<TransferLineCreateDto> Lines { get; set; } = new();
}

public class TransferLineCreateDto
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal TransferQuantity { get; set; }
}
