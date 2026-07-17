using Volo.Abp.Application.Dtos;

namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundOrderQueryDto — query DTO for filtering and paging outbound orders.
/// </summary>
public class OutboundOrderQueryDto : PagedAndSortedResultRequestDto
{
    /// <summary>Filter by outbound type value.</summary>
    public int? OutboundTypeValue { get; set; }

    /// <summary>Filter by outbound status value.</summary>
    public int? OutboundStatusValue { get; set; }

    /// <summary>Filter by warehouse ID.</summary>
    public Guid? WarehouseId { get; set; }

    /// <summary>Filter by emergency status.</summary>
    public bool? IsEmergency { get; set; }

    /// <summary>Keyword search — matches order number, warehouse code.</summary>
    public string? Keyword { get; set; }
}
