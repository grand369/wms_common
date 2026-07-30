namespace Wms.Web.Host.Dtos;

/// <summary>
/// DashboardStatsDto — overview statistics for the dashboard home page.
/// </summary>
public class DashboardStatsDto
{
    public decimal InventoryValue { get; set; }
    public int TodayInbound { get; set; }
    public int TodayOutbound { get; set; }
    public int PendingTasks { get; set; }
    public int AlertCount { get; set; }
}

/// <summary>
/// InboundTrendDto — daily inbound trend data.
/// </summary>
public class InboundTrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

/// <summary>
/// OutboundTrendDto — daily outbound trend data.
/// </summary>
public class OutboundTrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

/// <summary>
/// InventoryDistributionDto — inventory distribution by category.
/// </summary>
public class InventoryDistributionDto
{
    public string Category { get; set; } = string.Empty;
    public int Value { get; set; }
}

/// <summary>
/// TaskExecutionRateDto — task execution rate by type.
/// </summary>
public class TaskExecutionRateDto
{
    public string Name { get; set; } = string.Empty;
    public double Rate { get; set; }
    public int Total { get; set; }
    public int Completed { get; set; }
}

/// <summary>
/// DashboardAlertDto — alert information for the dashboard.
/// </summary>
public class DashboardAlertDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}