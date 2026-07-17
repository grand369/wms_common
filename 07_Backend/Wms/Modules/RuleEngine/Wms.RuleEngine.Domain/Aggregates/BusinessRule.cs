using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.RuleEngine.Domain.Enums;

namespace Wms.RuleEngine.Domain.Aggregates;

/// <summary>
/// BusinessRule Aggregate Root (AGG-26) — core aggregate for business rules.
/// Represents a configurable business rule that can be evaluated against context data.
/// </summary>
public class BusinessRule : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Rule name — unique business natural key.</summary>
    public string RuleName { get; private set; }

    /// <summary>Rule type — QualityInspection/PutawayStrategy/IssueStrategy/AlertThreshold.</summary>
    public RuleType RuleType { get; private set; }

    /// <summary>Rule condition — JSON string defining evaluation criteria.</summary>
    public string RuleCondition { get; private set; }

    /// <summary>Rule action — JSON string defining actions when condition is met.</summary>
    public string RuleAction { get; private set; }

    /// <summary>Rule version — incremented on each update.</summary>
    public int RuleVersion { get; private set; }

    /// <summary>Effectiveness status — Active/Inactive/Draft/Archived.</summary>
    public EffectiveStatus EffectiveStatus { get; private set; }

    /// <summary>Optional description.</summary>
    public string? Description { get; private set; }

    /// <summary>Optional effective from date.</summary>
    public DateTime? EffectiveFrom { get; private set; }

    /// <summary>Optional effective to date.</summary>
    public DateTime? EffectiveTo { get; private set; }

    /// <summary>Original creator user ID.</summary>
    public Guid? CreatedByUserId { get; private set; }

    /// <summary>Last modifier user ID.</summary>
    public Guid? LastModifiedByUserId { get; private set; }

    private BusinessRule() { }

    public BusinessRule(
        Guid id,
        string ruleName,
        RuleType ruleType,
        string ruleCondition,
        string ruleAction,
        string? description = null,
        DateTime? effectiveFrom = null,
        DateTime? effectiveTo = null,
        Guid? createdByUserId = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(ruleName))
            throw new BusinessException("WMS:RuleEngine:RuleNameRequired", "Rule name is required.");

        if (string.IsNullOrWhiteSpace(ruleCondition))
            throw new BusinessException("WMS:RuleEngine:ConditionRequired", "Rule condition is required.");

        if (string.IsNullOrWhiteSpace(ruleAction))
            throw new BusinessException("WMS:RuleEngine:ActionRequired", "Rule action is required.");

        RuleName = ruleName.Trim();
        RuleType = ruleType;
        RuleCondition = ruleCondition;
        RuleAction = ruleAction;
        RuleVersion = 1;
        EffectiveStatus = EffectiveStatus.Draft;
        Description = description;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        CreatedByUserId = createdByUserId;
    }

    /// <summary>
    /// Update rule condition JSON.
    /// </summary>
    public void UpdateCondition(string newCondition)
    {
        if (string.IsNullOrWhiteSpace(newCondition))
            throw new BusinessException("WMS:RuleEngine:ConditionRequired", "Rule condition is required.");

        RuleCondition = newCondition;
    }

    /// <summary>
    /// Update rule action JSON.
    /// </summary>
    public void UpdateAction(string newAction)
    {
        if (string.IsNullOrWhiteSpace(newAction))
            throw new BusinessException("WMS:RuleEngine:ActionRequired", "Rule action is required.");

        RuleAction = newAction;
    }

    /// <summary>
    /// Increment rule version — called when rule definition changes.
    /// </summary>
    public void IncrementVersion()
    {
        RuleVersion++;
    }

    /// <summary>
    /// Activate the rule — set status to Active.
    /// </summary>
    public void Activate()
    {
        if (EffectiveStatus == EffectiveStatus.Archived)
            throw new BusinessException("WMS:RuleEngine:CannotActivateArchived", "Cannot activate an archived rule.");

        EffectiveStatus = EffectiveStatus.Active;
    }

    /// <summary>
    /// Deactivate the rule — set status to Inactive.
    /// </summary>
    public void Deactivate()
    {
        EffectiveStatus = EffectiveStatus.Inactive;
    }

    /// <summary>
    /// Set effective period — validation first, then apply.
    /// </summary>
    public void SetEffectivePeriod(DateTime? from, DateTime? to)
    {
        if (from.HasValue && to.HasValue && from.Value >= to.Value)
            throw new BusinessException("WMS:RuleEngine:InvalidEffectivePeriod", "EffectiveFrom must be earlier than EffectiveTo.");

        EffectiveFrom = from;
        EffectiveTo = to;
    }

    /// <summary>
    /// Evaluate the rule against context data.
    /// Returns true if conditions are met (rule applies).
    /// </summary>
    public bool Evaluate(object contextData)
    {
        if (EffectiveStatus != EffectiveStatus.Active)
            return false;

        var now = DateTime.UtcNow;
        if (EffectiveFrom.HasValue && now < EffectiveFrom.Value)
            return false;

        if (EffectiveTo.HasValue && now > EffectiveTo.Value)
            return false;

        // For now, always return true for active rules within effective period.
        // Full evaluation engine would parse RuleCondition JSON and invoke expression.
        return true;
    }

    /// <summary>
    /// Update metadata about the rule name and description.
    /// </summary>
    public void UpdateMetadata(string ruleName, string? description)
    {
        if (string.IsNullOrWhiteSpace(ruleName))
            throw new BusinessException("WMS:RuleEngine:RuleNameRequired", "Rule name is required.");

        RuleName = ruleName.Trim();
        Description = description;
    }

    /// <summary>
    /// Set the last modified user.
    /// </summary>
    public void SetLastModifiedByUserId(Guid? userId)
    {
        LastModifiedByUserId = userId;
    }
}
