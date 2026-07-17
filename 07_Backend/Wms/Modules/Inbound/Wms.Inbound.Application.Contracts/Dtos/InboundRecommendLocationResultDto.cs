namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundRecommendLocationResultDto — putaway location recommendation result.
/// v1.0 placeholder — will be expanded in v1.1 with TaskCenter/RuleEngine integration.
/// </summary>
public class InboundRecommendLocationResultDto
{
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public decimal AvailableCapacity { get; set; }
    public decimal MaxCapacity { get; set; }
    public int Priority { get; set; }
    public string? ZoneName { get; set; }
}
