namespace Wms.Shared.Domain.Interfaces;

/// <summary>
/// Rule Engine Service Interface — extension point for custom rule execution.
/// Used by Inventory, Inbound, Outbound, Transfer modules for business rule validation.
/// </summary>
public interface IRuleEngineService
{
    /// <summary>
    /// Evaluates a business rule by name with the given context data.
    /// </summary>
    Task<bool> EvaluateRuleAsync(string ruleName, object contextData);

    /// <summary>
    /// Gets the list of effective rules for a given rule type.
    /// </summary>
    Task<List<string>> GetEffectiveRulesAsync(int ruleType);
}
