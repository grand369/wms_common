using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;
using Wms.RuleEngine.Domain.Repositories;

namespace Wms.RuleEngine.Domain.Services;

/// <summary>
/// RuleEngineDomainService (DS-12) — domain service for business rule evaluation
/// and industry package import.
/// </summary>
public class RuleEngineDomainService : DomainService
{
    private readonly IBusinessRuleRepository _businessRuleRepository;
    private readonly IIndustryPackageRepository _industryPackageRepository;

    public RuleEngineDomainService(
        IBusinessRuleRepository businessRuleRepository,
        IIndustryPackageRepository industryPackageRepository)
    {
        _businessRuleRepository = businessRuleRepository;
        _industryPackageRepository = industryPackageRepository;
    }

    /// <summary>
    /// Evaluate a business rule by name with context data.
    /// Returns true if conditions are met.
    /// </summary>
    public async Task<bool> EvaluateRuleAsync(string ruleName, object contextData)
    {
        var rule = await _businessRuleRepository.FindByRuleNameAsync(ruleName);
        if (rule == null)
            throw new BusinessException("WMS:RuleEngine:RuleNotFound", $"Rule '{ruleName}' not found.");

        return rule.Evaluate(contextData);
    }

    /// <summary>
    /// Get names of all effective rules for a given rule type.
    /// </summary>
    public async Task<List<string>> GetEffectiveRulesAsync(RuleType ruleType)
    {
        var rules = await _businessRuleRepository.GetEffectiveRulesAsync(ruleType);
        return rules.Select(r => r.RuleName).ToList();
    }

    /// <summary>
    /// Import an industry package — parse its content and create BusinessRule entities.
    /// Returns the list of imported rule names.
    /// </summary>
    public async Task<List<string>> ImportIndustryPackageAsync(Guid packageId)
    {
        var package = await _industryPackageRepository.GetAsync(packageId);
        if (package.IsImported)
            throw new BusinessException("WMS:RuleEngine:PackageAlreadyImported",
                $"Industry package '{package.PackageName}' has already been imported.");

        var importedRuleNames = new List<string>();

        try
        {
            // Parse package content JSON — expected format: { "rules": [{ "name":"...", "type":0, "condition":"...", "action":"..." }] }
            using var doc = JsonDocument.Parse(package.PackageContent);
            var rulesElement = doc.RootElement.GetProperty("rules");

            foreach (var ruleElement in rulesElement.EnumerateArray())
            {
                var ruleName = ruleElement.GetProperty("name").GetString()
                    ?? throw new BusinessException("WMS:RuleEngine:InvalidPackageContent", "Rule name missing in package content.");
                var ruleTypeValue = ruleElement.GetProperty("type").GetInt32();
                var ruleCondition = ruleElement.GetProperty("condition").GetString()
                    ?? throw new BusinessException("WMS:RuleEngine:InvalidPackageContent", "Rule condition missing in package content.");
                var ruleAction = ruleElement.GetProperty("action").GetString()
                    ?? throw new BusinessException("WMS:RuleEngine:InvalidPackageContent", "Rule action missing in package content.");

                if (!RuleType.TryFromValue(ruleTypeValue, out var ruleType))
                    throw new BusinessException("WMS:RuleEngine:InvalidRuleType",
                        $"Rule type value {ruleTypeValue} is not valid.");

                // Check for existing rule by same name
                var existing = await _businessRuleRepository.FindByRuleNameAsync(ruleName);
                if (existing != null)
                {
                    existing.UpdateCondition(ruleCondition);
                    existing.UpdateAction(ruleAction);
                    existing.IncrementVersion();
                    await _businessRuleRepository.UpdateAsync(existing);
                    importedRuleNames.Add(ruleName);
                    continue;
                }

                var rule = new BusinessRule(
                    GuidGenerator.Create(),
                    ruleName,
                    ruleType,
                    ruleCondition,
                    ruleAction,
                    description: $"Imported from package '{package.PackageName}'"
                );
                rule.Activate();

                await _businessRuleRepository.InsertAsync(rule);
                importedRuleNames.Add(ruleName);
            }

            package.MarkImported();
            await _industryPackageRepository.UpdateAsync(package);

            return importedRuleNames;
        }
        catch (JsonException ex)
        {
            throw new BusinessException("WMS:RuleEngine:InvalidPackageContent",
                $"Failed to parse package content: {ex.Message}");
        }
    }
}
