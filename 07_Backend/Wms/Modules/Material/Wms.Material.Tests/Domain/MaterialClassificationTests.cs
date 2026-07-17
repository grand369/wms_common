using Shouldly;
using Wms.Material.Domain.Aggregates;

namespace Wms.Material.Tests.Domain;

/// <summary>
/// Material Classification Domain Tests — tests aggregate root creation, code/name setting, parent update.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class MaterialClassificationTests
{
    [Fact]
    public void Create_Classification_Should_Set_All_Properties()
    {
        // Arrange & Act
        var classification = new MaterialClassification(
            Guid.NewGuid(),
            "CLS-01",
            "金属材料",
            null,
            1);

        // Assert
        classification.ClassificationCode.ShouldBe("CLS-01");
        classification.ClassificationName.ShouldBe("金属材料");
        classification.ParentClassificationId.ShouldBeNull();
        classification.ClassificationLevel.ShouldBe(1);
    }

    [Fact]
    public void Create_Classification_WithParent_Should_Set_ParentId()
    {
        var parentId = Guid.NewGuid();
        var classification = new MaterialClassification(
            Guid.NewGuid(),
            "CLS-01-01",
            "钢板",
            parentId,
            2);

        classification.ParentClassificationId.ShouldBe(parentId);
        classification.ClassificationLevel.ShouldBe(2);
    }

    [Fact]
    public void SetClassificationCode_Should_Update_Code()
    {
        var classification = CreateSampleClassification();
        classification.SetClassificationCode("CLS-02");
        classification.ClassificationCode.ShouldBe("CLS-02");
    }

    [Fact]
    public void SetClassificationCode_Empty_Should_Throw()
    {
        var classification = CreateSampleClassification();
        Should.Throw<ArgumentException>(() => classification.SetClassificationCode(""));
    }

    [Fact]
    public void SetClassificationName_Should_Update_Name()
    {
        var classification = CreateSampleClassification();
        classification.SetClassificationName("铝材料");
        classification.ClassificationName.ShouldBe("铝材料");
    }

    [Fact]
    public void SetClassificationName_Empty_Should_Throw()
    {
        var classification = CreateSampleClassification();
        Should.Throw<ArgumentException>(() => classification.SetClassificationName(""));
    }

    [Fact]
    public void UpdateParent_Should_Update_ParentId_And_Level()
    {
        var classification = CreateSampleClassification();
        var newParentId = Guid.NewGuid();
        classification.UpdateParent(newParentId, 2);

        classification.ParentClassificationId.ShouldBe(newParentId);
        classification.ClassificationLevel.ShouldBe(2);
    }

    [Fact]
    public void UpdateParent_ToRoot_Should_Set_NullParent_And_Level1()
    {
        var parentId = Guid.NewGuid();
        var classification = new MaterialClassification(
            Guid.NewGuid(),
            "CLS-01-01",
            "钢板",
            parentId,
            2);

        classification.UpdateParent(null, 1);
        classification.ParentClassificationId.ShouldBeNull();
        classification.ClassificationLevel.ShouldBe(1);
    }

    private static MaterialClassification CreateSampleClassification()
    {
        return new MaterialClassification(
            Guid.NewGuid(),
            "CLS-01",
            "金属材料",
            null,
            1);
    }
}
