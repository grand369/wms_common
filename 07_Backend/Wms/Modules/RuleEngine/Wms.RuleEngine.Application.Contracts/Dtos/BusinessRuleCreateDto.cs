using System.ComponentModel.DataAnnotations;

namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// BusinessRuleCreateDto — input DTO for creating a business rule.
/// </summary>
public class BusinessRuleCreateDto
{
    [Required]
    [MaxLength(100)]
    public string RuleName { get; set; }

    public int RuleTypeValue { get; set; }

    [Required]
    public string RuleCondition { get; set; }

    [Required]
    public string RuleAction { get; set; }

    public string? Description { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
