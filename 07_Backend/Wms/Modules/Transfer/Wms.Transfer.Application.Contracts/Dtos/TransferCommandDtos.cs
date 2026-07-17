using System;
using System.Collections.Generic;

namespace Wms.Transfer.Application.Contracts.Dtos;

/// <summary>Command DTOs for Transfer business operations</summary>

/// <summary>Confirm outbound command — API-TF-008</summary>
public class ConfirmTransferOutboundCommandDto
{
    public List<OutboundConfirmLineDto> Lines { get; set; } = new();
}

public class OutboundConfirmLineDto
{
    public int LineNo { get; set; }
    public decimal ConfirmedQuantity { get; set; }
}

/// <summary>Confirm inbound command — API-TF-009</summary>
public class ConfirmTransferInboundCommandDto
{
    public List<InboundConfirmLineDto> Lines { get; set; } = new();
}

public class InboundConfirmLineDto
{
    public int LineNo { get; set; }
    public decimal ConfirmedQuantity { get; set; }
}
