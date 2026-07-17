using Volo.Abp.Domain.Entities.Auditing;
using Wms.Material.Domain.Events;

namespace Wms.Material.Domain.Aggregates;

/// <summary>
/// Material Classification Aggregate Root (AGG-05, ENT-05) — represents a classification node in the material classification tree.
/// Inherits FullAuditedAggregateRoot for ABP audit fields and soft delete.
/// Supports hierarchical tree structure via ParentClassificationId.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialClassification : FullAuditedAggregateRoot<Guid>
{
    /// <summary>分类编码（唯一）</summary>
    public string ClassificationCode { get; private set; } = string.Empty;

    /// <summary>分类名称</summary>
    public string ClassificationName { get; private set; } = string.Empty;

    /// <summary>父分类ID（null = 根分类）</summary>
    public Guid? ParentClassificationId { get; private set; }

    /// <summary>分类层级（1=一级）</summary>
    public int ClassificationLevel { get; private set; } = 1;

    /// <summary>属性模板ID</summary>
    public Guid? AttributeTemplateId { get; private set; }

    public MaterialClassification() { }

    public MaterialClassification(
        Guid id,
        string classificationCode,
        string classificationName,
        Guid? parentClassificationId = null,
        int classificationLevel = 1,
        Guid? attributeTemplateId = null) : base(id)
    {
        SetClassificationCode(classificationCode);
        SetClassificationName(classificationName);
        ParentClassificationId = parentClassificationId;
        ClassificationLevel = classificationLevel;
        AttributeTemplateId = attributeTemplateId;

        AddLocalEvent(new MaterialClassificationCreatedEvent
        {
            ClassificationId = Id,
            ClassificationCode = ClassificationCode,
            ClassificationName = ClassificationName,
            ClassificationLevel = ClassificationLevel
        });
    }

    public MaterialClassification SetClassificationCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Classification code cannot be empty.", nameof(code));
        ClassificationCode = code.Trim();
        return this;
    }

    public MaterialClassification SetClassificationName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Classification name cannot be empty.", nameof(name));
        ClassificationName = name.Trim();
        return this;
    }

    public MaterialClassification UpdateParent(Guid? parentClassificationId, int classificationLevel)
    {
        ParentClassificationId = parentClassificationId;
        ClassificationLevel = classificationLevel;
        return this;
    }

    /// <summary>Sets the attribute template ID for this classification.</summary>
    public MaterialClassification SetAttributeTemplateId(Guid? attributeTemplateId)
    {
        AttributeTemplateId = attributeTemplateId;
        return this;
    }
}
