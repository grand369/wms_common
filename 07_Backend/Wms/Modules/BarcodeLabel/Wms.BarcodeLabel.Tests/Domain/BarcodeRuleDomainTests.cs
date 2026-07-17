using System;
using FluentAssertions;
using Volo.Abp;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Xunit;

namespace Wms.BarcodeLabel.Tests.Domain;

public class BarcodeRuleDomainTests
{
    private BarcodeRule CreateSampleRule()
    {
        return new BarcodeRule(
            Guid.NewGuid(),
            "TestRule",
            BarcodeType.Material,
            BarcodeFormat.Code128,
            "MAT-{SEQ}-{DATE:yyyyMMdd}",
            "MAT");
    }

    [Fact]
    public void GenerateNextCode_IncrementsSeqCounter()
    {
        var rule = CreateSampleRule();
        var code1 = rule.GenerateNextCode();
        var code2 = rule.GenerateNextCode();

        rule.SeqCounter.Should().Be(2);
        code1.Should().Contain("000001");
        code2.Should().Contain("000002");
    }

    [Fact]
    public void GenerateNextCode_ContainsPrefix()
    {
        var rule = CreateSampleRule();
        var code = rule.GenerateNextCode();

        code.Should().StartWith("MAT-");
    }

    [Fact]
    public void GenerateNextCode_ContainsDate()
    {
        var rule = CreateSampleRule();
        var code = rule.GenerateNextCode();

        var todayDate = DateTime.UtcNow.ToString("yyyyMMdd");
        code.Should().Contain(todayDate);
    }

    [Fact]
    public void Deactivate_ChangesIsActiveToFalse()
    {
        var rule = CreateSampleRule();

        rule.IsActive.Should().BeTrue();
        rule.Deactivate();
        rule.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsBusinessException()
    {
        var rule = CreateSampleRule();
        rule.Deactivate();

        var act = () => rule.Deactivate();
        act.Should().Throw<BusinessException>()
            .WithMessage("*already inactive*");
    }

    [Fact]
    public void UpdatePattern_ChangesCodePattern()
    {
        var rule = CreateSampleRule();
        rule.UpdatePattern("MAT-{PREFIX}-{SEQ}");

        var code = rule.GenerateNextCode();
        code.Should().StartWith("MAT-MAT-");
    }

    [Fact]
    public void UpdatePattern_WithEmptyString_ThrowsBusinessException()
    {
        var rule = CreateSampleRule();

        var act = () => rule.UpdatePattern(string.Empty);
        act.Should().Throw<BusinessException>()
            .WithMessage("*Code pattern cannot be empty*");
    }

    [Fact]
    public void UpdatePattern_WithNull_ThrowsBusinessException()
    {
        var rule = CreateSampleRule();

        var act = () => rule.UpdatePattern(null!);
        act.Should().Throw<BusinessException>();
    }
}
