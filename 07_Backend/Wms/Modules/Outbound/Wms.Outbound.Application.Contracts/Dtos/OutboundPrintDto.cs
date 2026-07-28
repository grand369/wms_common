namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundPrintType — type of document to print.
/// </summary>
public enum OutboundPrintType
{
    /// <summary>Outbound order document.</summary>
    Order = 1,
    
    /// <summary>Packing list.</summary>
    PackingList = 2,
    
    /// <summary>Address label.</summary>
    AddressLabel = 3
}

/// <summary>
/// OutboundPrintDto — print request DTO.
/// </summary>
public class OutboundPrintDto
{
    /// <summary>Type of document to print.</summary>
    public OutboundPrintType PrintType { get; set; }

    /// <summary>Whether to include barcode.</summary>
    public bool IncludeBarcode { get; set; } = true;

    /// <summary>Number of copies.</summary>
    public int Copies { get; set; } = 1;
}
