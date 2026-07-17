using System.ComponentModel.DataAnnotations;

namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// BusinessRuleUpdateDto — input DTO for updating a business rule.
/// </summary>
public class BusinessRuleUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string RuleName { get; set; }

    [Required]
    public string RuleCondition { get; set; }

    [Required]
    public string RuleAction { get; set; }

    public string? Description { get; set; }

    public int EffectiveStatusValue { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
