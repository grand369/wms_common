namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// BusinessRuleOutputDto — output DTO for business rule display.
/// </summary>
public class BusinessRuleOutputDto
{
    public Guid Id { get; set; }

    public string RuleName { get; set; }

    public int RuleTypeValue { get; set; }

    public string RuleCondition { get; set; }

    public string RuleAction { get; set; }

    public int RuleVersion { get; set; }

    public int EffectiveStatusValue { get; set; }

    public string? Description { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime CreationTime { get; set; }
}
