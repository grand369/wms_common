using Volo.Abp.Domain.Repositories;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;

namespace Wms.RuleEngine.Domain.Repositories;

/// <summary>
/// IBusinessRuleRepository (REP-22) — custom query methods for BusinessRule aggregate.
/// </summary>
public interface IBusinessRuleRepository : IRepository<BusinessRule, Guid>
{
    /// <summary>Find business rule by rule name (unique business key).</summary>
    Task<BusinessRule?> FindByRuleNameAsync(string ruleName);

    /// <summary>Get business rules by rule type.</summary>
    Task<List<BusinessRule>> GetByRuleTypeAsync(RuleType ruleType);

    /// <summary>Get business rule by rule name and version.</summary>
    Task<BusinessRule?> GetByVersionAsync(string ruleName, int version);

    /// <summary>Get all effective (active) rules for a given rule type.</summary>
    Task<List<BusinessRule>> GetEffectiveRulesAsync(RuleType ruleType);
}
