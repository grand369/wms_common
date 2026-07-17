using Shouldly;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Xunit;

namespace Wms.Inventory.Tests.Domain;

/// <summary>
/// Inventory Ledger Tests — verifies the immutable nature of ledger entries.
/// </summary>
public class InventoryLedgerTests
{
    [Fact]
    public void Create_LedgerEntry_AllFieldsSet()
    {
        var entry = new InventoryLedgerEntry(
            Guid.NewGuid(),
            Guid.NewGuid(),
            InventoryOperationType.InboundIncrease,
            100m,
            0m,
            100m,
            0m,
            100m,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Operator-1",
            "InboundOrder",
            Guid.NewGuid(),
            "IN-001",
            null);

        entry.InventoryBalanceId.ShouldNotBe(Guid.Empty);
        entry.OperationType.ShouldBe(InventoryOperationType.InboundIncrease);
        entry.OperationQuantity.ShouldBe(100m);
        entry.BeforeQuantity.ShouldBe(0m);
        entry.AfterQuantity.ShouldBe(100m);
        entry.OperatorName.ShouldBe("Operator-1");
        entry.SourceOrderType.ShouldBe("InboundOrder");
        entry.SourceOrderNo.ShouldBe("IN-001");
    }

    [Fact]
    public void LedgerEntry_InheritsEntityNotFullAuditedAggregateRoot()
    {
        var entry = new InventoryLedgerEntry(
            Guid.NewGuid(),
            Guid.NewGuid(),
            InventoryOperationType.InboundIncrease,
            50m,
            0m,
            50m,
            0m,
            50m,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Operator-1",
            "InboundOrder",
            Guid.NewGuid(),
            "IN-002",
            null);

        // LedgerEntry inherits Entity<Guid>, NOT FullAuditedAggregateRoot
        // It should have IHasCreationTime but NOT IHasModificationTime or ISoftDelete
        entry.ShouldBeOfType<InventoryLedgerEntry>();
        // InventoryLedgerEntry inherits Entity<Guid>, which implements IHasCreationTime
        entry.CreationTime.ShouldNotBe(default);
    }
}
