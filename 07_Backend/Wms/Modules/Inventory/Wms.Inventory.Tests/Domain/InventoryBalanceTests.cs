using Shouldly;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.StateMachines;
using Wms.Shared.Domain.Enums;
using Xunit;

namespace Wms.Inventory.Tests.Domain;

/// <summary>
/// Inventory Balance Domain Tests — core tests for the platform's heart entity.
/// Covers: creation, quantity changes, reservations, freezes, available calculation, status changes.
/// </summary>
public class InventoryBalanceTests
{
    private InventoryBalance CreateTestBalance()
    {
        return new InventoryBalance(
            Guid.NewGuid(),
            Guid.NewGuid(), "MAT-001",
            Guid.NewGuid(), "WH-001",
            Guid.NewGuid(), "LOC-001",
            "BATCH-001",
            InventoryStatus.Available);
    }

    [Fact]
    public void Create_InventoryBalance_ShouldHaveZeroQuantities()
    {
        var balance = CreateTestBalance();

        balance.Quantity.ShouldBe(0m);
        balance.ReservedQuantity.ShouldBe(0m);
        balance.FrozenQuantity.ShouldBe(0m);
        balance.InTransitQuantity.ShouldBe(0m);
        balance.AvailableQuantity.ShouldBe(0m);
        balance.InventoryStatus.ShouldBe(InventoryStatus.Available);
        balance.MaterialCode.ShouldBe("MAT-001");
    }

    [Fact]
    public void ApplyQuantityChange_InboundIncrease_ShouldIncreaseQuantity()
    {
        var balance = CreateTestBalance();
        var result = balance.ApplyQuantityChange(
            InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());

        balance.Quantity.ShouldBe(100m);
        balance.AvailableQuantity.ShouldBe(100m);
        result.AfterQuantity.ShouldBe(100m);
        result.ChangeQuantity.ShouldBe(100m);
        result.LedgerEntry.ShouldNotBeNull();
        result.LedgerEntry.OperationType.ShouldBe(InventoryOperationType.InboundIncrease);
    }

    [Fact]
    public void ApplyQuantityChange_OutboundDecrease_ShouldDecreaseQuantity()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());

        var result = balance.ApplyQuantityChange(
            InventoryOperationType.OutboundDecrease, 30m, "OutboundOrder", Guid.NewGuid());

        balance.Quantity.ShouldBe(70m);
        balance.AvailableQuantity.ShouldBe(70m);
        result.BeforeQuantity.ShouldBe(100m);
        result.AfterQuantity.ShouldBe(70m);
    }

    [Fact]
    public void ApplyQuantityChange_NegativeInventoryNotAllowed_ShouldThrowException()
    {
        var balance = CreateTestBalance();

        Should.Throw<BusinessException>(() =>
        {
            balance.ApplyQuantityChange(
                InventoryOperationType.OutboundDecrease, 10m, "OutboundOrder", Guid.NewGuid(),
                allowNegativeInventory: false);
        });
    }

    [Fact]
    public void ApplyQuantityChange_NegativeInventoryAllowed_ShouldAllowNegative()
    {
        var balance = CreateTestBalance();

        var result = balance.ApplyQuantityChange(
            InventoryOperationType.OutboundDecrease, 10m, "OutboundOrder", Guid.NewGuid(),
            allowNegativeInventory: true);

        balance.Quantity.ShouldBe(-10m);
        balance.AvailableQuantity.ShouldBe(-10m);
    }

    [Fact]
    public void ReserveQuantity_ShouldIncreaseReservedAndDecreaseAvailable()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());

        balance.ReserveQuantity(30m, "OutboundOrder", Guid.NewGuid());

        balance.ReservedQuantity.ShouldBe(30m);
        balance.AvailableQuantity.ShouldBe(70m); // 100 - 30 - 0
        balance.Quantity.ShouldBe(100m); // Quantity unchanged
    }

    [Fact]
    public void ReserveQuantity_ExceedAvailable_ShouldThrowException()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 50m, "InboundOrder", Guid.NewGuid());

        Should.Throw<BusinessException>(() =>
        {
            balance.ReserveQuantity(60m, "OutboundOrder", Guid.NewGuid());
        });
    }

    [Fact]
    public void ReleaseReservation_ShouldDecreaseReservedAndIncreaseAvailable()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());
        balance.ReserveQuantity(30m, "OutboundOrder", Guid.NewGuid());

        balance.ReleaseReservation(20m, "OutboundOrder", Guid.NewGuid());

        balance.ReservedQuantity.ShouldBe(10m);
        balance.AvailableQuantity.ShouldBe(90m); // 100 - 10 - 0
    }

    [Fact]
    public void FreezeQuantity_ShouldIncreaseFrozenAndDecreaseAvailable()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());

        balance.FreezeQuantity(40m, "FreezeOrder", Guid.NewGuid());

        balance.FrozenQuantity.ShouldBe(40m);
        balance.AvailableQuantity.ShouldBe(60m); // 100 - 0 - 40
    }

    [Fact]
    public void UnfreezeQuantity_ShouldDecreaseFrozenAndIncreaseAvailable()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());
        balance.FreezeQuantity(40m, "FreezeOrder", Guid.NewGuid());

        balance.UnfreezeQuantity(20m, "FreezeOrder", Guid.NewGuid());

        balance.FrozenQuantity.ShouldBe(20m);
        balance.AvailableQuantity.ShouldBe(80m); // 100 - 0 - 20
    }

    [Fact]
    public void AvailableQuantity_ShouldBeCalculatedCorrectly()
    {
        var balance = CreateTestBalance();
        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());
        balance.ReserveQuantity(20m, "OutboundOrder", Guid.NewGuid());
        balance.FreezeQuantity(15m, "FreezeOrder", Guid.NewGuid());

        // Available = Quantity - Reserved - Frozen = 100 - 20 - 15 = 65
        balance.AvailableQuantity.ShouldBe(65m);
        balance.Quantity.ShouldBe(100m);
        balance.ReservedQuantity.ShouldBe(20m);
        balance.FrozenQuantity.ShouldBe(15m);
    }

    [Fact]
    public void ChangeStatus_ValidTransition_ShouldChangeStatus()
    {
        var balance = CreateTestBalance();
        balance.ChangeStatus(InventoryStatus.QualityHold);

        balance.InventoryStatus.ShouldBe(InventoryStatus.QualityHold);
    }

    [Fact]
    public void ChangeStatus_InvalidTransition_ShouldThrowException()
    {
        var balance = CreateTestBalance();
        // Available → Scrapped is not a valid transition per SM-04
        Should.Throw<BusinessException>(() =>
        {
            balance.ChangeStatus(InventoryStatus.Scrapped);
        });
    }

    [Fact]
    public void UpdateExpiryInfo_ShouldUpdateDates()
    {
        var balance = CreateTestBalance();
        var expiryDate = DateTime.UtcNow.AddDays(365);
        var productionDate = DateTime.UtcNow;

        balance.UpdateExpiryInfo(expiryDate, productionDate);

        balance.ExpiryDate.ShouldBe(expiryDate);
        balance.ProductionDate.ShouldBe(productionDate);
    }

    [Fact]
    public void UpdateCost_ShouldUpdateCostFields()
    {
        var balance = CreateTestBalance();
        var supplierId = Guid.NewGuid();

        balance.UpdateCost(10.5m, supplierId, "Supplier-A");

        balance.UnitCost.ShouldBe(10.5m);
        balance.SupplierId.ShouldBe(supplierId);
        balance.SupplierName.ShouldBe("Supplier-A");
    }
}
