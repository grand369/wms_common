using System.ComponentModel.DataAnnotations;

namespace Wms.RuleEngine.Application.Contracts.Dtos;

/// <summary>
/// IndustryPackageCreateDto — input DTO for creating an industry package.
/// </summary>
public class IndustryPackageCreateDto
{
    [Required]
    [MaxLength(100)]
    public string PackageName { get; set; }

    public int IndustryTypeValue { get; set; }

    [Required]
    public string PackageContent { get; set; }

    public string? Description { get; set; }
}
