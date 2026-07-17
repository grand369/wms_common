using BenchmarkDotNet.Attributes;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;
using Notif = Wms.Notification.Domain.Aggregates.Notification;

namespace Wms.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
public class NotificationBatchBenchmark
{
    [Params(100, 1000, 10000)]
    public int IterationCount { get; set; }

    private NotificationTemplate _template = null!;
    private Dictionary<string, string> _templateVariables = null!;
    private Notif _notification = null!;

    [GlobalSetup]
    public void Setup()
    {
        _template = new NotificationTemplate(
            Guid.NewGuid(),
            "StockAlert",
            NotificationType.Alert,
            NotificationChannel.Internal,
            "库存预警: 物料 {MaterialCode} 在仓库 {WarehouseCode} 库位 {LocationCode} 的库存已降至 {CurrentQty}，低于安全阈值 {ThresholdQty}。请及时补货。",
            description: "Stock alert template for performance testing");

        _templateVariables = new Dictionary<string, string>
        {
            ["MaterialCode"] = "MAT-001",
            ["WarehouseCode"] = "WH-MAIN",
            ["LocationCode"] = "LOC-A-01-01",
            ["CurrentQty"] = "50",
            ["ThresholdQty"] = "100"
        };

        _notification = new Notif(
            Guid.NewGuid(),
            NotificationType.Alert,
            NotificationChannel.Internal,
            "库存预警",
            "物料 MAT-001 库存不足",
            Guid.NewGuid(),
            "TestUser",
            NotificationPriority.High,
            sourceEvent: "StockLowAlert",
            sourceModule: "Inventory",
            correlationId: Guid.NewGuid());
    }

    [Benchmark]
    public string TemplateRender_Simple()
    {
        string result = string.Empty;
        for (int i = 0; i < IterationCount; i++)
        {
            result = _template.RenderTemplate(_templateVariables);
        }

        return result;
    }

    [Benchmark]
    public string TemplateRender_WithLargerVariables()
    {
        var variables = new Dictionary<string, string>
        {
            ["MaterialCode"] = "MAT-001",
            ["MaterialName"] = "精密轴承-6205",
            ["WarehouseCode"] = "WH-MAIN",
            ["WarehouseName"] = "主仓库A区",
            ["LocationCode"] = "LOC-A-01-01",
            ["CurrentQty"] = "50",
            ["ThresholdQty"] = "100",
            ["DeficitQty"] = "50",
            ["SupplierName"] = "优质轴承供应商有限公司",
            ["ContactPhone"] = "13800138000"
        };

        var template = new NotificationTemplate(
            Guid.NewGuid(),
            "DetailedAlert",
            NotificationType.Alert,
            NotificationChannel.Internal,
            "库存预警详细报告\n物料: {MaterialCode} ({MaterialName})\n仓库: {WarehouseCode} ({WarehouseName})\n库位: {LocationCode}\n当前库存: {CurrentQty}\n安全阈值: {ThresholdQty}\n缺额: {DeficitQty}\n建议供应商: {SupplierName}\n联系电话: {ContactPhone}",
            description: "Detailed alert with more variables");

        string result = string.Empty;
        for (int i = 0; i < IterationCount; i++)
        {
            result = template.RenderTemplate(variables);
        }

        return result;
    }

    [Benchmark]
    public void Notification_FullLifecycle()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var notification = new Notif(
                Guid.NewGuid(),
                NotificationType.Alert,
                NotificationChannel.Internal,
                "库存预警",
                $"物料 MAT-{i:D6} 库存不足",
                Guid.NewGuid(),
                "TestUser",
                NotificationPriority.High,
                sourceEvent: "StockLowAlert",
                sourceModule: "Inventory");

            notification.MarkAsSent();
            notification.MarkAsRead();
        }
    }

    [Benchmark]
    public void Notification_MarkAsSentRead()
    {
        // Reset notification state for each iteration
        for (int i = 0; i < IterationCount; i++)
        {
            _notification.MarkAsSent();
            _notification.MarkAsRead();
            _notification.MarkAsFailed("Reset for next iteration");
            _notification.Retry();
        }
    }

    [Benchmark]
    public void NotificationTemplate_CreateAndRender()
    {
        string result = string.Empty;
        for (int i = 0; i < IterationCount; i++)
        {
            var template = new NotificationTemplate(
                Guid.NewGuid(),
                $"Template-{i}",
                NotificationType.Alert,
                NotificationChannel.Internal,
                "Alert: {MaterialCode} at {WarehouseCode} has {CurrentQty} units",
                description: null);

            result = template.RenderTemplate(_templateVariables);
        }

        result.ToString();
    }


    [Benchmark]
    public void NotificationRule_CreateAndUpdate()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var rule = new NotificationRule(
                Guid.NewGuid(),
                $"Rule-{i}",
                "StockLowAlert",
                "Inventory",
                NotificationChannel.Internal,
                NotificationType.Alert,
                NotificationPriority.High,
                targetRole: "WarehouseManager",
                templateId: Guid.NewGuid(),
                isEnabled: true);

            rule.Disable();
            rule.UpdateTargetChannel(NotificationChannel.Email);
            rule.Enable();
        }
    }
}
