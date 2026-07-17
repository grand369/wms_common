using BenchmarkDotNet.Attributes;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Wms.Inventory.Domain.ValueObjects;

namespace Wms.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
public class InventoryBalanceBenchmark
{
    [Params(100, 1000, 10000)]
    public int IterationCount { get; set; }

    private InventoryBalance _balance = null!;
    private readonly Guid _sourceOrderId = Guid.NewGuid();
    private const decimal ChangeQty = 10m;

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
            10000m,
            "PO",
            _sourceOrderId,
            sourceOrderNo: "PO-001");
    }

    [Benchmark]
    public InventoryChangeResult ApplyQuantityChange_InboundIncrease()
    {
        var result = null as InventoryChangeResult;
        for (int i = 0; i < IterationCount; i++)
        {
            result = _balance.ApplyQuantityChange(
                InventoryOperationType.InboundIncrease,
                ChangeQty,
                "PO",
                _sourceOrderId,
                sourceOrderNo: "PO-001");
        }

        return result!;
    }

    [Benchmark]
    public void ReserveQuantity()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            _balance.ReserveQuantity(ChangeQty, "SalesOrder", _sourceOrderId);
            _balance.ReleaseReservation(ChangeQty, "SalesOrder", _sourceOrderId);
        }
    }

    [Benchmark]
    public void ReleaseReservation()
    {
        _balance.ReserveQuantity(ChangeQty, "SalesOrder", _sourceOrderId);
        for (int i = 0; i < IterationCount; i++)
        {
            _balance.ReleaseReservation(ChangeQty, "SalesOrder", _sourceOrderId);
            _balance.ReserveQuantity(ChangeQty, "SalesOrder", _sourceOrderId);
        }

        _balance.ReleaseReservation(ChangeQty, "SalesOrder", _sourceOrderId);
    }

    [Benchmark]
    public void FreezeQuantity()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            _balance.FreezeQuantity(ChangeQty, "Audit", _sourceOrderId);
            _balance.UnfreezeQuantity(ChangeQty, "Audit", _sourceOrderId);
        }
    }

    [Benchmark]
    public void UnfreezeQuantity()
    {
        _balance.FreezeQuantity(ChangeQty, "Audit", _sourceOrderId);
        for (int i = 0; i < IterationCount; i++)
        {
            _balance.UnfreezeQuantity(ChangeQty, "Audit", _sourceOrderId);
            _balance.FreezeQuantity(ChangeQty, "Audit", _sourceOrderId);
        }

        _balance.UnfreezeQuantity(ChangeQty, "Audit", _sourceOrderId);
    }

    [Benchmark]
    public decimal AvailableQuantity()
    {
        var result = 0m;
        for (int i = 0; i < IterationCount; i++)
        {
            result = _balance.AvailableQuantity;
        }

        return result;
    }
}
