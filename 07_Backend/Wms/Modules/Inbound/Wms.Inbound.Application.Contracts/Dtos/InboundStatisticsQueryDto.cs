namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundStatisticsQueryDto — input DTO for querying inbound order statistics.
/// </summary>
public class InboundStatisticsQueryDto
{
    /// <summary>Start date for filtering.</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>End date for filtering.</summary>
    public DateTime? EndDate { get; set; }
}
