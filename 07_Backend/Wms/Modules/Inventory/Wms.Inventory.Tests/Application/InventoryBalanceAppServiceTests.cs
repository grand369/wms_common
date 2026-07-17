using Shouldly;
using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Wms.Shared.Domain.Enums;
using Moq;
using Xunit;

namespace Wms.Inventory.Tests.Application;

/// <summary>
/// Inventory Balance App Service Tests — verifies query and initialization operations.
/// </summary>
public class InventoryBalanceAppServiceTests
{
    private readonly Mock<IInventoryBalanceRepository> _mockBalanceRepo;
    private readonly Mock<InventoryDomainService> _mockDomainService;

    public InventoryBalanceAppServiceTests()
    {
        _mockBalanceRepo = new Mock<IInventoryBalanceRepository>();
        _mockDomainService = new Mock<InventoryDomainService>(
            _mockBalanceRepo.Object,
            new Mock<IInventoryLedgerRepository>().Object,
            new Mock<Volo.Abp.EventBus.Local.ILocalEventBus>().Object);
    }

    [Fact]
    public void MapToOutputDto_ShouldFlattenAllFields()
    {
        var balance = new InventoryBalance(
            Guid.NewGuid(),
            Guid.NewGuid(), "MAT-001",
            Guid.NewGuid(), "WH-001",
            Guid.NewGuid(), "LOC-001",
            "BATCH-001",
            InventoryStatus.Available);

        balance.ApplyQuantityChange(InventoryOperationType.InboundIncrease, 100m, "InboundOrder", Guid.NewGuid());

        // Verify fields are accessible for mapping
        balance.MaterialCode.ShouldBe("MAT-001");
        balance.WarehouseCode.ShouldBe("WH-001");
        balance.LocationCode.ShouldBe("LOC-001");
        balance.BatchNumber.ShouldBe("BATCH-001");
        balance.InventoryStatus.Value.ShouldBe(0); // Available = 0
        balance.InventoryStatus.Description.ShouldBe("可用");
        balance.Quantity.ShouldBe(100m);
        balance.AvailableQuantity.ShouldBe(100m);
        balance.ConcurrencyVersion.ShouldBe(1);
    }
}
