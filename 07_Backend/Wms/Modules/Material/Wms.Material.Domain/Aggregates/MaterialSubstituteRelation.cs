using Volo.Abp.Domain.Entities.Auditing;

namespace Wms.Material.Domain.Aggregates;

/// <summary>
/// Material Substitute Relation — child entity nested within Material aggregate (ENT-04a).
/// Inherits FullAuditedEntity<Guid> for ABP audit fields.
/// Represents an alternative material that can substitute the original material.
/// (AGG-04, Phase 3 DDD Design)
/// </summary>
public class MaterialSubstituteRelation : FullAuditedEntity<Guid>
{
    /// <summary>原物料ID（本物料 ID）</summary>
    public Guid OriginalMaterialId { get; private set; }

    /// <summary>替代料ID</summary>
    public Guid SubstituteMaterialId { get; private set; }

    /// <summary>替代料编码（冗余）</summary>
    public string SubstituteMaterialCode { get; private set; } = string.Empty;

    /// <summary>替代料名称（冗余）</summary>
    public string SubstituteMaterialName { get; private set; } = string.Empty;

    /// <summary>替代优先级（1=首选替代）</summary>
    public int SubstitutePriority { get; private set; }

    /// <summary>替代比例（替代料用量 / 原料用量）</summary>
    public decimal SubstituteRatio { get; private set; }

    public MaterialSubstituteRelation() { }

    public MaterialSubstituteRelation(
        Guid id,
        Guid originalMaterialId,
        Guid substituteMaterialId,
        string substituteMaterialCode,
        string substituteMaterialName = "",
        int substitutePriority = 1,
        decimal substituteRatio = 1.0m)
    {
        Id = id;
        OriginalMaterialId = originalMaterialId;
        SubstituteMaterialId = substituteMaterialId;
        SubstituteMaterialCode = substituteMaterialCode ?? throw new ArgumentNullException(nameof(substituteMaterialCode));
        SubstituteMaterialName = substituteMaterialName ?? string.Empty;
        SubstitutePriority = substitutePriority;
        SubstituteRatio = substituteRatio;
    }
}
