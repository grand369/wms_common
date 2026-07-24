namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundStatisticsDto — output DTO for inbound order statistics.
/// </summary>
public class InboundStatisticsDto
{
    /// <summary>Total number of inbound orders.</summary>
    public int TotalCount { get; set; }

    /// <summary>Number of pending inbound orders (Draft or Confirmed).</summary>
    public int PendingCount { get; set; }

    /// <summary>Number of completed inbound orders.</summary>
    public int CompletedCount { get; set; }

    /// <summary>Number of inbound orders created today.</summary>
    public int TodayCount { get; set; }
}
