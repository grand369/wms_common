using System;
using Shouldly;
using Volo.Abp.Testing;
using Wms.CycleCount.Domain.Aggregates;
using Wms.CycleCount.Domain.Enums;

namespace Wms.CycleCount.Tests.Domain;

public class CycleCountPlanTests : AbpIntegratedTest<WmsCycleCountTestModule>
{
    private CycleCountPlan CreateSamplePlan()
    {
        return new CycleCountPlan(
            Guid.NewGuid(), "CC-2026-001", CountMethod.Full,
            Guid.NewGuid(), "WH-01", DateTime.UtcNow,
            true, 2.0m, true);
    }

    [Fact]
    public void Plan_Starts_As_Planned()
    {
        var plan = CreateSamplePlan();
        plan.CountStatus.ShouldBe(CountStatus.Planned);
    }

    [Fact]
    public void Plan_Can_Start_Counting()
    {
        var plan = CreateSamplePlan();
        plan.StartCounting();
        plan.CountStatus.ShouldBe(CountStatus.InProgress);
    }

    [Fact]
    public void InProgress_Can_Complete()
    {
        var plan = CreateSamplePlan();
        plan.StartCounting();
        plan.CompleteCounting();
        plan.CountStatus.ShouldBe(CountStatus.Completed);
    }

    [Fact]
    public void Completed_Can_Close()
    {
        var plan = CreateSamplePlan();
        plan.StartCounting();
        plan.CompleteCounting();
        plan.Close();
        plan.CountStatus.ShouldBe(CountStatus.Closed);
    }

    [Fact]
    public void AddItem_Increases_Count()
    {
        var plan = CreateSamplePlan();
        plan.AddItem(Guid.NewGuid(), "LOC-01", Guid.NewGuid(), "MAT-001");
        plan.Items.Count.ShouldBe(1);
    }

    [Fact]
    public void SubmitCountData_Updates_ActualQuantity()
    {
        var plan = CreateSamplePlan();
        var itemId = Guid.NewGuid();
        plan.AddItem(Guid.NewGuid(), "LOC-01", itemId, "MAT-001");
        plan.Items[0].SetSystemQuantity(100);
        plan.SubmitCountData(plan.Items[0].Id, 98);
        plan.Items[0].ActualQuantity.ShouldBe(98);
        plan.Items[0].DifferenceQuantity.ShouldBe(-2);
    }

    [Fact]
    public void Recount_Resets_ActualQuantity()
    {
        var plan = CreateSamplePlan();
        plan.AddItem(Guid.NewGuid(), "LOC-01", Guid.NewGuid(), "MAT-001");
        plan.Items[0].SetSystemQuantity(100);
        plan.SubmitCountData(plan.Items[0].Id, 98);
        plan.RecountItem(plan.Items[0].Id);
        plan.Items[0].ActualQuantity.ShouldBeNull();
    }

    [Fact]
    public void NonPlanned_Cannot_Start()
    {
        var plan = CreateSamplePlan();
        plan.StartCounting();
        Should.Throw<BusinessException>(() => plan.StartCounting());
    }
}
