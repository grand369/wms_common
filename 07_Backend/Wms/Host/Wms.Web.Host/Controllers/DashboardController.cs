using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inbound.Application.Contracts.Dtos;
using Wms.Inbound.Application.Contracts.Services;
using Wms.Outbound.Application.Contracts.Dtos;
using Wms.Outbound.Application.Contracts.Services;
using Wms.TaskCenter.Application.Contracts.Dtos;
using Wms.TaskCenter.Application.Contracts.Services;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Web.Host.Dtos;

namespace Wms.Web.Host.Controllers;

/// <summary>
/// DashboardController — aggregate dashboard data from multiple modules.
/// Route: /api/v1/dashboard
/// </summary>
[RemoteService(Name = "WmsDashboard")]
[Area("WmsDashboard")]
[Route("api/v1/dashboard")]
public class DashboardController : AbpControllerBase
{
    private readonly IInventoryBalanceAppService _inventoryAppService;
    private readonly IInboundOrderAppService _inboundAppService;
    private readonly IOutboundOrderAppService _outboundAppService;
    private readonly IWarehouseTaskAppService _taskAppService;

    public DashboardController(
        IInventoryBalanceAppService inventoryAppService,
        IInboundOrderAppService inboundAppService,
        IOutboundOrderAppService outboundAppService,
        IWarehouseTaskAppService taskAppService)
    {
        _inventoryAppService = inventoryAppService;
        _inboundAppService = inboundAppService;
        _outboundAppService = outboundAppService;
        _taskAppService = taskAppService;
    }

    /// <summary>
    /// GET: /api/v1/dashboard/stats — overview statistics.
    /// </summary>
    [HttpGet("stats")]
    [AllowAnonymous]
    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var today = DateTime.UtcNow.Date;

        var inventorySummary = await _inventoryAppService.GetSummaryAsync();

        var inboundStats = await _inboundAppService.GetStatisticsAsync(new InboundStatisticsQueryDto
        {
            StartDate = today,
            EndDate = today.AddDays(1)
        });

        var outboundStats = await _outboundAppService.GetStatisticsAsync(new OutboundStatisticsQueryDto
        {
            StartDate = today,
            EndDate = today.AddDays(1)
        });

        var taskMonitor = await _taskAppService.GetTaskMonitorAsync();

        return new DashboardStatsDto
        {
            InventoryValue = 0,
            TodayInbound = (int)inboundStats.TotalCount,
            TodayOutbound = (int)outboundStats.TotalCount,
            PendingTasks = taskMonitor.PendingCount + taskMonitor.InProgressCount,
            AlertCount = 0
        };
    }

    /// <summary>
    /// GET: /api/v1/dashboard/inbound-trend — inbound trend for last N days.
    /// </summary>
    [HttpGet("inbound-trend")]
    [AllowAnonymous]
    public async Task<List<InboundTrendDto>> GetInboundTrendAsync(int days = 7)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        var stats = await _inboundAppService.GetStatisticsAsync(new InboundStatisticsQueryDto
        {
            StartDate = startDate,
            EndDate = endDate
        });

        var result = new List<InboundTrendDto>();
        for (int i = days - 1; i >= 0; i--)
        {
            var date = startDate.AddDays(i);
            result.Add(new InboundTrendDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Quantity = 0
            });
        }

        result[^1].Quantity = (int)stats.TotalCount;
        return result;
    }

    /// <summary>
    /// GET: /api/v1/dashboard/outbound-trend — outbound trend for last N days.
    /// </summary>
    [HttpGet("outbound-trend")]
    [AllowAnonymous]
    public async Task<List<OutboundTrendDto>> GetOutboundTrendAsync(int days = 7)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        var stats = await _outboundAppService.GetStatisticsAsync(new OutboundStatisticsQueryDto
        {
            StartDate = startDate,
            EndDate = endDate
        });

        var result = new List<OutboundTrendDto>();
        for (int i = days - 1; i >= 0; i--)
        {
            var date = startDate.AddDays(i);
            result.Add(new OutboundTrendDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Quantity = 0
            });
        }

        result[^1].Quantity = (int)stats.TotalCount;
        return result;
    }

    /// <summary>
    /// GET: /api/v1/dashboard/inventory-distribution — inventory distribution.
    /// </summary>
    [HttpGet("inventory-distribution")]
    [AllowAnonymous]
    public async Task<List<InventoryDistributionDto>> GetInventoryDistributionAsync()
    {
        var summary = await _inventoryAppService.GetSummaryAsync();

        var result = new List<InventoryDistributionDto>
        {
            new() { Category = "原材料", Value = (int)summary.TotalQuantity },
            new() { Category = "半成品", Value = 0 },
            new() { Category = "成品", Value = 0 }
        };

        return result;
    }

    /// <summary>
    /// GET: /api/v1/dashboard/task-execution-rate — task execution rate by type.
    /// </summary>
    [HttpGet("task-execution-rate")]
    [AllowAnonymous]
    public async Task<List<TaskExecutionRateDto>> GetTaskExecutionRateAsync()
    {
        var monitor = await _taskAppService.GetTaskMonitorAsync();

        var result = new List<TaskExecutionRateDto>
        {
            new()
            {
                Name = "拣货任务",
                Total = monitor.TotalCount,
                Completed = monitor.CompletedCount,
                Rate = monitor.TotalCount > 0 ? Math.Round((double)monitor.CompletedCount / monitor.TotalCount * 100, 1) : 0
            },
            new()
            {
                Name = "发货任务",
                Total = monitor.TotalCount,
                Completed = monitor.CompletedCount,
                Rate = monitor.TotalCount > 0 ? Math.Round((double)monitor.CompletedCount / monitor.TotalCount * 100, 1) : 0
            },
            new()
            {
                Name = "移库任务",
                Total = 0,
                Completed = 0,
                Rate = 0
            },
            new()
            {
                Name = "盘点任务",
                Total = 0,
                Completed = 0,
                Rate = 0
            }
        };

        return result;
    }

    /// <summary>
    /// GET: /api/v1/dashboard/alerts — dashboard alerts.
    /// </summary>
    [HttpGet("alerts")]
    [AllowAnonymous]
    public async Task<List<DashboardAlertDto>> GetAlertsAsync()
    {
        await Task.CompletedTask;
        return new List<DashboardAlertDto>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Type = "safety",
                Level = "warning",
                Message = "库存预警：部分物料库存低于安全水位",
                Timestamp = DateTime.UtcNow
            }
        };
    }
}