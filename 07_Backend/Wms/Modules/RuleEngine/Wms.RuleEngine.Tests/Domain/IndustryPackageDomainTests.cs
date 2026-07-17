using Shouldly;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;
using Xunit;

namespace Wms.RuleEngine.Tests.Domain;

/// <summary>
/// IndustryPackageDomainTests — covers UpdateContent(), IncrementVersion(), MarkImported().
/// 3 tests.
/// </summary>
public class IndustryPackageDomainTests
{
    private IndustryPackage CreateTestPackage()
    {
        return new IndustryPackage(
            Guid.NewGuid(),
            "Automotive-QC-2024",
            IndustryType.Automotive,
            "{\"rules\":[{\"name\":\"QC_Auto\",\"type\":0,\"condition\":\"{}\",\"action\":\"{}\"}]}",
            "Automotive quality control package"
        );
    }

    [Fact]
    public void Create_IndustryPackage_ShouldHaveCorrectDefaults()
    {
        var package = CreateTestPackage();

        package.PackageName.ShouldBe("Automotive-QC-2024");
        package.IndustryType.ShouldBe(IndustryType.Automotive);
        package.PackageVersion.ShouldBe(1);
        package.IsImported.ShouldBeFalse();
    }

    [Fact]
    public void UpdateContent_ShouldChangePackageContent()
    {
        var package = CreateTestPackage();
        var newContent = "{\"rules\":[{\"name\":\"QC_Auto_V2\",\"type\":1,\"condition\":\"{}\",\"action\":\"{}\"}]}";

        package.UpdateContent(newContent);

        package.PackageContent.ShouldBe(newContent);
    }

    [Fact]
    public void IncrementVersion_ShouldIncrementByOne()
    {
        var package = CreateTestPackage();
        var initialVersion = package.PackageVersion;

        package.IncrementVersion();

        package.PackageVersion.ShouldBe(initialVersion + 1);
    }

    [Fact]
    public void MarkImported_ShouldSetIsImportedToTrue()
    {
        var package = CreateTestPackage();

        package.MarkImported();

        package.IsImported.ShouldBeTrue();
    }
}
