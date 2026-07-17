namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// RuleEvaluateDto — input DTO for evaluating a business rule with context data.
/// </summary>
public class RuleEvaluateDto
{
    public string RuleName { get; set; }

    public object ContextData { get; set; }
}
