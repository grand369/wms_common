using System;

namespace Wms.Transfer.Application.Contracts.Dtos;

/// <summary>TransferOrder Query DTO — used for API-TF-001 list filtering</summary>
public class TransferOrderQueryDto
{
    public int? TransferStatusValue { get; set; }
    public int? TransferTypeValue { get; set; }
    public Guid? SourceWarehouseId { get; set; }
    public Guid? TargetWarehouseId { get; set; }
    public string? TransferOrderNo { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
