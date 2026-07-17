using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.RuleEngine.Domain.Enums;

namespace Wms.RuleEngine.Domain.Aggregates;

/// <summary>
/// IndustryPackage Aggregate Root (AGG-27) — core aggregate for industry configuration packages.
/// Represents a set of industry-specific rules that can be imported into the system.
/// </summary>
public class IndustryPackage : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Package name.</summary>
    public string PackageName { get; private set; }

    /// <summary>Package version — incremented when content changes.</summary>
    public int PackageVersion { get; private set; }

    /// <summary>Industry type — Automotive/Electronics/Food/Pharmaceutical/General.</summary>
    public IndustryType IndustryType { get; private set; }

    /// <summary>Package content — JSON string with rule definitions.</summary>
    public string PackageContent { get; private set; }

    /// <summary>Optional description.</summary>
    public string? Description { get; private set; }

    /// <summary>Whether the package has been imported into the system.</summary>
    public bool IsImported { get; private set; }

    private IndustryPackage() { }

    public IndustryPackage(
        Guid id,
        string packageName,
        IndustryType industryType,
        string packageContent,
        string? description = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(packageName))
            throw new BusinessException("WMS:RuleEngine:PackageNameRequired", "Package name is required.");

        if (string.IsNullOrWhiteSpace(packageContent))
            throw new BusinessException("WMS:RuleEngine:PackageContentRequired", "Package content is required.");

        PackageName = packageName.Trim();
        IndustryType = industryType;
        PackageContent = packageContent;
        PackageVersion = 1;
        Description = description;
        IsImported = false;
    }

    /// <summary>
    /// Update package content JSON.
    /// </summary>
    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new BusinessException("WMS:RuleEngine:PackageContentRequired", "Package content is required.");

        PackageContent = newContent;
    }

    /// <summary>
    /// Increment package version.
    /// </summary>
    public void IncrementVersion()
    {
        PackageVersion++;
    }

    /// <summary>
    /// Mark package as imported — idempotent.
    /// </summary>
    public void MarkImported()
    {
        IsImported = true;
    }
}
