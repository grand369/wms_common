using System;
using FluentAssertions;
using Volo.Abp;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Xunit;

namespace Wms.BarcodeLabel.Tests.Domain;

public class LabelTemplateDomainTests
{
    private LabelTemplate CreateSampleTemplate()
    {
        return new LabelTemplate(
            Guid.NewGuid(),
            "InboundLabel",
            LabelTemplateType.Inbound,
            "<template><barcode>${code}</barcode></template>",
            "GS1-128");
    }

    [Fact]
    public void NewTemplate_StartsAtVersion1()
    {
        var template = CreateSampleTemplate();
        template.TemplateVersion.Should().Be(1);
    }

    [Fact]
    public void UpdateContent_IncrementsVersion()
    {
        var template = CreateSampleTemplate();
        template.UpdateContent("<template><barcode>${code}</barcode><text>${name}</text></template>");

        template.TemplateVersion.Should().Be(2);
        template.TemplateContent.Should().Contain("${name}");
    }

    [Fact]
    public void IncrementVersion_IncreasesVersionByOne()
    {
        var template = CreateSampleTemplate();
        template.IncrementVersion();
        template.TemplateVersion.Should().Be(2);

        template.IncrementVersion();
        template.TemplateVersion.Should().Be(3);
    }

    [Fact]
    public void Deactivate_ChangesIsActiveToFalse()
    {
        var template = CreateSampleTemplate();

        template.IsActive.Should().BeTrue();
        template.Deactivate();
        template.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsBusinessException()
    {
        var template = CreateSampleTemplate();
        template.Deactivate();

        var act = () => template.Deactivate();
        act.Should().Throw<BusinessException>()
            .WithMessage("*already inactive*");
    }

    [Fact]
    public void UpdateContent_WithEmptyString_ThrowsBusinessException()
    {
        var template = CreateSampleTemplate();

        var act = () => template.UpdateContent(string.Empty);
        act.Should().Throw<BusinessException>()
            .WithMessage("*Template content cannot be empty*");
    }
}
