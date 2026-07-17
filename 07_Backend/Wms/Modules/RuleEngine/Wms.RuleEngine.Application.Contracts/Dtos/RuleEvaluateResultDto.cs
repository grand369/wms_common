namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// RuleEvaluateResultDto — output DTO for rule evaluation result.
/// </summary>
public class RuleEvaluateResultDto
{
    public string RuleName { get; set; }

    public bool Result { get; set; }

    public DateTime EvaluatedAt { get; set; }
}
