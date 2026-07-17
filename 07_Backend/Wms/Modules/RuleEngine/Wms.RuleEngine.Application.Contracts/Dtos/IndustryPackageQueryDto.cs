namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// IndustryPackageQueryDto — query DTO for filtering and paging industry packages.
/// </summary>
public class IndustryPackageQueryDto
{
    public int? IndustryTypeValue { get; set; }

    public int SkipCount { get; set; } = 0;

    public int MaxResultCount { get; set; } = 20;
}
