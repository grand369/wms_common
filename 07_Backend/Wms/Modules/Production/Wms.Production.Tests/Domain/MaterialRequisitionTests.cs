using System;
using Shouldly;
using Volo.Abp.Testing;
using Wms.Production.Domain.Aggregates;
using Wms.Production.Domain.Enums;

namespace Wms.Production.Tests.Domain;

public class MaterialRequisitionTests : AbpIntegratedTest<WmsProductionTestModule>
{
    private MaterialRequisition CreateSample()
    {
        var req = new MaterialRequisition(
            Guid.NewGuid(), "MR-2026-001", Guid.NewGuid(), "PO-2026-001",
            Guid.NewGuid(), "WH-01");
        req.AddLine(1, Guid.NewGuid(), "MAT-001", 100);
        req.AddLine(2, Guid.NewGuid(), "MAT-002", 50);
        return req;
    }

    [Fact]
    public void Requisition_Starts_As_Draft()
    {
        var req = CreateSample();
        req.RequisitionStatus.ShouldBe(RequisitionStatus.Draft);
    }

    [Fact]
    public void Draft_Can_Submit()
    {
        var req = CreateSample();
        req.Submit();
        req.RequisitionStatus.ShouldBe(RequisitionStatus.Submitted);
    }

    [Fact]
    public void Draft_Can_Cancel()
    {
        var req = CreateSample();
        req.Cancel();
        req.RequisitionStatus.ShouldBe(RequisitionStatus.Cancelled);
    }

    [Fact]
    public void AddLine_Increases_Count()
    {
        var req = CreateSample();
        req.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void IssueLine_Updates_IssuedQuantity()
    {
        var req = CreateSample();
        req.IssueLine(1, 80);
        req.Lines[0].IssuedQuantity.ShouldBe(80);
    }

    [Fact]
    public void OverIssue_Beyond_10_Pct_Throws()
    {
        var req = CreateSample();
        Should.Throw<BusinessException>(() => req.IssueLine(1, 120));
    }

    [Fact]
    public void Submitted_Cannot_Cancel()
    {
        var req = CreateSample();
        req.Submit();
        Should.Throw<BusinessException>(() => req.Cancel());
    }
}
