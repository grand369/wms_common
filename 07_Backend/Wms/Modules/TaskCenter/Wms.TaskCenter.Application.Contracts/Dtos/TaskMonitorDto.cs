namespace Wms.TaskCenter.Application.Contracts.Dtos;

/// <summary>
/// TaskMonitorDto — task monitoring statistics (API-TC-015).
/// </summary>
public class TaskMonitorDto
{
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public int ExceptionCount { get; set; }
    public int TotalCount { get; set; }
}
