namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// BusinessRuleQueryDto — query DTO for filtering and paging business rules.
/// </summary>
public class BusinessRuleQueryDto
{
    public int? RuleTypeValue { get; set; }

    public int? EffectiveStatusValue { get; set; }

    public string? RuleName { get; set; }

    public int SkipCount { get; set; } = 0;

    public int MaxResultCount { get; set; } = 20;
}
