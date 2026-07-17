using System;
using System.Collections.Generic;

namespace Wms.Transfer.Application.Contracts.Dtos;

/// <summary>TransferOrder Output DTO — used for GET responses</summary>
public class TransferOrderOutputDto
{
    public Guid Id { get; set; }
    public string TransferOrderNo { get; set; }
    public int TransferTypeValue { get; set; }
    public string TransferTypeDescription { get; set; }
    public int TransferStatusValue { get; set; }
    public string TransferStatusDescription { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public string SourceWarehouseCode { get; set; }
    public Guid TargetWarehouseId { get; set; }
    public string TargetWarehouseCode { get; set; }
    public int ApprovalStatusValue { get; set; }
    public string ApprovalStatusDescription { get; set; }
    public bool IsCrossCompany { get; set; }
    public string? Remark { get; set; }
    public List<TransferLineOutputDto> Lines { get; set; } = new();
}

public class TransferLineOutputDto
{
    public Guid Id { get; set; }
    public int LineNo { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; }
    public decimal TransferQuantity { get; set; }
    public decimal OutboundConfirmedQuantity { get; set; }
    public decimal InboundConfirmedQuantity { get; set; }
}
