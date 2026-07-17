using Volo.Abp.Application.Dtos;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundOrderQueryDto — query DTO for filtering and paging inbound orders.
/// </summary>
public class InboundOrderQueryDto : PagedAndSortedResultRequestDto
{
    /// <summary>Filter by inbound type value.</summary>
    public int? InboundTypeValue { get; set; }

    /// <summary>Filter by inbound status value.</summary>
    public int? InboundStatusValue { get; set; }

    /// <summary>Filter by warehouse ID.</summary>
    public Guid? WarehouseId { get; set; }

    /// <summary>Keyword search — matches order number, warehouse code, supplier name.</summary>
    public string? Keyword { get; set; }
}
