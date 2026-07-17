namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// IndustryPackageOutputDto — output DTO for industry package display.
/// </summary>
public class IndustryPackageOutputDto
{
    public Guid Id { get; set; }

    public string PackageName { get; set; }

    public int PackageVersion { get; set; }

    public int IndustryTypeValue { get; set; }

    public string PackageContent { get; set; }

    public string? Description { get; set; }

    public bool IsImported { get; set; }

    public DateTime CreationTime { get; set; }
}
