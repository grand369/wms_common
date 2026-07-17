using Wms.RuleEngine.Domain.Enums;
using Wms.RuleEngine.Domain.Services;
using Wms.Shared.Domain.Interfaces;

namespace Wms.RuleEngine.Application.Services;

/// <summary>
/// RuleEngineService — cross-module service implementing IRuleEngineService from Shared Kernel.
/// Wraps RuleEngineDomainService for cross-module synchronous calls (OHS pattern).
/// </summary>
public class RuleEngineService : IRuleEngineService
{
    private readonly RuleEngineDomainService _ruleEngineDomainService;

    public RuleEngineService(RuleEngineDomainService ruleEngineDomainService)
    {
        _ruleEngineDomainService = ruleEngineDomainService;
    }

    public async Task<bool> EvaluateRuleAsync(string ruleName, object contextData)
    {
        return await _ruleEngineDomainService.EvaluateRuleAsync(ruleName, contextData);
    }

    public async Task<List<string>> GetEffectiveRulesAsync(int ruleType)
    {
        var type = RuleType.FromValue(ruleType);
        return await _ruleEngineDomainService.GetEffectiveRulesAsync(type);
    }
}
