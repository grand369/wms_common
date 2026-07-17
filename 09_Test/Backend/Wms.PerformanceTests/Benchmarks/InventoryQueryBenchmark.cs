using BenchmarkDotNet.Attributes;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Shared.Domain.Enums;

namespace Wms.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
public class InventoryQueryBenchmark
{
    [Params(100, 1000, 10000)]
    public int IterationCount { get; set; }

    private InventoryBalance _balance = null!;
    private InventoryAdjustment _adjustment = null!;
    private InventoryAdjustmentLine _sampleLine = null!;

    [GlobalSetup]
    public void Setup()
    {
        _balance = new InventoryBalance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "MAT-001",
            Guid.NewGuid(),
            "WH-001",
            Guid.NewGuid(),
            "LOC-001",
            "BATCH-001",
            InventoryStatus.Available);

        _balance.ApplyQuantityChange(
            InventoryOperationType.InboundIncrease,
            5000m,
            "PO",
            Guid.NewGuid(),
            sourceOrderNo: "PO-001");

        _balance.ReserveQuantity(1000m, "SalesOrder", Guid.NewGuid());
        _balance.FreezeQuantity(500m, "Audit", Guid.NewGuid());

        _adjustment = new InventoryAdjustment(
            Guid.NewGuid(),
            "ADJ-PERF-001",
            AdjustmentType.Gain,
            "Performance test adjustment",
            _balance.WarehouseId,
            _balance.WarehouseCode,
            remark: null);

        _sampleLine = new InventoryAdjustmentLine(
            Guid.NewGuid(),
            _adjustment.Id,
            lineNo: 1,
            Guid.NewGuid(),
            "MAT-001",
            "Test Material",
            adjustmentQuantity: 100m,
            Guid.NewGuid(),
            "LOC-001",
            batchNumber: "BATCH-001",
            InventoryStatus.Available,
            InventoryStatus.Available,
            reason: "Count correction");
    }

    [Benchmark]
    public decimal ReadAvailableQuantity()
    {
        decimal result = 0m;
        for (int i = 0; i < IterationCount; i++)
        {
            result += _balance.AvailableQuantity;
        }

        return result;
    }

    [Benchmark]
    public decimal ReadAllProperties()
    {
        decimal sum = 0m;
        for (int i = 0; i < IterationCount; i++)
        {
            sum += _balance.Quantity;
            sum += _balance.ReservedQuantity;
            sum += _balance.FrozenQuantity;
            sum += _balance.AvailableQuantity;
            sum += _balance.InTransitQuantity;
        }

        return sum;
    }

    [Benchmark]
    public void ChangeStatus_AllTransitions()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            _balance.ChangeStatus(InventoryStatus.Reserved);
            _balance.ChangeStatus(InventoryStatus.Frozen);
            _balance.ChangeStatus(InventoryStatus.Available);
        }
    }

    [Benchmark]
    public void UpdateExpiryAndCost()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            _balance.UpdateExpiryInfo(
                expiryDate: DateTime.UtcNow.AddMonths(12),
                productionDate: DateTime.UtcNow.AddMonths(-1));

            _balance.UpdateCost(
                unitCost: 125.50m,
                supplierId: Guid.NewGuid(),
                supplierName: "TestSupplier");
        }
    }

    [Benchmark]
    public void Adjustment_AddLine()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var line = new InventoryAdjustmentLine(
                Guid.NewGuid(),
                _adjustment.Id,
                lineNo: i + 1,
                Guid.NewGuid(),
                $"MAT-{i:D6}",
                $"Test Material {i}",
                adjustmentQuantity: 10m,
                Guid.NewGuid(),
                $"LOC-{i:D6}",
                batchNumber: "BATCH-001",
                InventoryStatus.Available,
                InventoryStatus.Available,
                reason: "Test");

            _adjustment.AddLine(line);
        }
    }

    [Benchmark]
    public void Adjustment_SubmitExecute()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var adj = new InventoryAdjustment(
                Guid.NewGuid(),
                $"ADJ-{DateTime.Now:yyyyMMdd}-{i:D6}",
                AdjustmentType.Gain,
                "Performance test",
                _balance.WarehouseId,
                _balance.WarehouseCode,
                remark: null);

            adj.AddLine(new InventoryAdjustmentLine(
                Guid.NewGuid(),
                adj.Id,
                lineNo: 1,
                _balance.MaterialId,
                _balance.MaterialCode,
                "Test Material",
                adjustmentQuantity: 50m,
                _balance.LocationId,
                _balance.LocationCode,
                batchNumber: _balance.BatchNumber,
                InventoryStatus.Available,
                InventoryStatus.Available,
                reason: "System recalculation"));

            adj.Submit();
            adj.Approve();
            adj.Execute();
        }
    }
}
