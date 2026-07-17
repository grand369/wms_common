using System;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Aggregates;

/// <summary>
/// BarcodeRule Aggregate Root (AGG-21) — defines barcode generation rules.
/// </summary>
public class BarcodeRule : FullAuditedAggregateRoot<Guid>
{
    public string RuleName { get; private set; }
    public BarcodeType BarcodeType { get; private set; }
    public BarcodeFormat BarcodeFormat { get; private set; }
    public string CodePattern { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public int SeqCounter { get; private set; }
    public string? Prefix { get; private set; }

    private BarcodeRule() { }

    public BarcodeRule(
        Guid id,
        string ruleName,
        BarcodeType barcodeType,
        BarcodeFormat barcodeFormat,
        string codePattern,
        string? prefix = null,
        string? description = null)
        : base(id)
    {
        RuleName = ruleName ?? throw new ArgumentNullException(nameof(ruleName));
        BarcodeType = barcodeType ?? throw new ArgumentNullException(nameof(barcodeType));
        BarcodeFormat = barcodeFormat ?? throw new ArgumentNullException(nameof(barcodeFormat));
        CodePattern = codePattern ?? throw new ArgumentNullException(nameof(codePattern));
        Prefix = prefix;
        Description = description;
        IsActive = true;
        SeqCounter = 0;
    }

    /// <summary>
    /// Generate the next barcode code based on the code pattern.
    /// Increments the sequence counter and formats the result.
    /// Pattern supports: {PREFIX}, {SEQ}, {DATE:format}, {TYPE}
    /// </summary>
    public string GenerateNextCode()
    {
        SeqCounter++;

        var result = CodePattern
            .Replace("{PREFIX}", Prefix ?? string.Empty)
            .Replace("{SEQ}", SeqCounter.ToString("D6"))
            .Replace("{DATE:yyyyMMdd}", DateTime.UtcNow.ToString("yyyyMMdd"))
            .Replace("{DATE:yyMMddHHmmss}", DateTime.UtcNow.ToString("yyMMddHHmmss"))
            .Replace("{TYPE}", BarcodeType.Name);

        return result;
    }

    /// <summary>Deactivate this barcode rule.</summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new BusinessException("WMS:BarcodeLabel:BarcodeRuleAlreadyInactive",
                $"Barcode rule '{RuleName}' is already inactive.");

        IsActive = false;
    }

    /// <summary>Update the code pattern for this rule.</summary>
    public void UpdatePattern(string newPattern)
    {
        if (string.IsNullOrWhiteSpace(newPattern))
            throw new BusinessException("WMS:BarcodeLabel:InvalidPattern",
                "Code pattern cannot be empty.");

        CodePattern = newPattern;
    }

    /// <summary>Update the prefix for this rule.</summary>
    public void UpdatePrefix(string? prefix)
    {
        Prefix = prefix;
    }

    /// <summary>Update rule name and description.</summary>
    public void UpdateInfo(string ruleName, string? description)
    {
        if (string.IsNullOrWhiteSpace(ruleName))
            throw new BusinessException("WMS:BarcodeLabel:InvalidRuleName",
                "Rule name cannot be empty.");

        RuleName = ruleName;
        Description = description;
    }

    /// <summary>Set active status.</summary>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}
