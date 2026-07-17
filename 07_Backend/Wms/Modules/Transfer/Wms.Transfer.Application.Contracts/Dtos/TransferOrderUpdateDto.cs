using System;

namespace Wms.Transfer.Application.Contracts.Dtos;

/// <summary>TransferOrder Update DTO — used for API-TF-004</summary>
public class TransferOrderUpdateDto
{
    public string? Remark { get; set; }
    public bool IsCrossCompany { get; set; }
}
