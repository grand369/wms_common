namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundStatisticsDto — API-OB-015 outbound statistics response.
/// </summary>
public class OutboundStatisticsDto
{
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int CompletedCount { get; set; }
    public int TodayCount { get; set; }
}

/// <summary>
/// OutboundStatisticsQueryDto — query parameters for outbound statistics.
/// </summary>
public class OutboundStatisticsQueryDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
