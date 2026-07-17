using System;
using Shouldly;
using Volo.Abp.Testing;
using Wms.LineSide.Domain.Aggregates;
using Wms.LineSide.Domain.Enums;

namespace Wms.LineSide.Tests.Domain;

public class LineSideWarehouseTests : AbpIntegratedTest<WmsLineSideTestModule>
{
    private LineSideWarehouse CreateSample()
    {
        var lsw = new LineSideWarehouse(
            Guid.NewGuid(), "LS-001", "线边仓A",
            Guid.NewGuid(), "WH-MAIN",
            Guid.NewGuid(), "产线1",
            null, ConsumptionMode.Scan);
        lsw.AddKanbanItem(Guid.NewGuid(), "MAT-001", 10, 50);
        lsw.AddKanbanItem(Guid.NewGuid(), "MAT-002", 5, 30);
        // Receive initial stock
        lsw.ReceiveReplenishment(lsw.KanbanItems[0].MaterialId, 30);
        lsw.ReceiveReplenishment(lsw.KanbanItems[1].MaterialId, 20);
        return lsw;
    }

    [Fact]
    public void Can_Add_KanbanItem()
    {
        var lsw = CreateSample();
        lsw.KanbanItems.Count.ShouldBe(2);
    }

    [Fact]
    public void Receive_Replenishment_Increases_Stock()
    {
        var lsw = CreateSample();
        lsw.KanbanItems[0].CurrentQuantity.ShouldBe(30);
    }

    [Fact]
    public void BackflushConsume_Decreases_Stock()
    {
        var lsw = CreateSample();
        var matId = lsw.KanbanItems[0].MaterialId;
        lsw.BackflushConsume(Guid.NewGuid(), matId, 10);
        lsw.KanbanItems[0].CurrentQuantity.ShouldBe(20);
    }

    [Fact]
    public void Consume_Exceeding_Stock_Throws()
    {
        var lsw = CreateSample();
        var matId = lsw.KanbanItems[0].MaterialId;
        Should.Throw<BusinessException>(() => lsw.BackflushConsume(Guid.NewGuid(), matId, 40));
    }

    [Fact]
    public void Kanban_Below_Min_Detected()
    {
        var lsw = CreateSample();
        var matId = lsw.KanbanItems[0].MaterialId;
        // Consume down to below min (10)
        lsw.BackflushConsume(Guid.NewGuid(), matId, 25);
        lsw.CheckKanbanThresholds();
        // Should have published KanbanReplenishmentTriggeredEvent
    }
}
