using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Wms.BarcodeLabel.Domain.Repositories;
using Wms.Shared.Domain.Helpers;

namespace Wms.BarcodeLabel.Domain.Services;

/// <summary>
/// DS-10: BarcodeLabelDomainService — domain logic for barcode generation, parsing, and print task creation.
/// </summary>
public class BarcodeLabelDomainService : DomainService
{
    private readonly IBarcodeRuleRepository _barcodeRuleRepository;
    private readonly IPrintTaskRepository _printTaskRepository;
    private readonly ILabelTemplateRepository _labelTemplateRepository;

    public BarcodeLabelDomainService(
        IBarcodeRuleRepository barcodeRuleRepository,
        IPrintTaskRepository printTaskRepository,
        ILabelTemplateRepository labelTemplateRepository)
    {
        _barcodeRuleRepository = barcodeRuleRepository;
        _printTaskRepository = printTaskRepository;
        _labelTemplateRepository = labelTemplateRepository;
    }

    /// <summary>
    /// Generate a barcode using the specified rule pattern and a reference ID.
    /// </summary>
    public async Task<(BarcodeRule Rule, string GeneratedCode)> GenerateBarcodeAsync(Guid ruleId, string referenceId)
    {
        var rule = await _barcodeRuleRepository.GetAsync(ruleId);

        if (!rule.IsActive)
            throw new BusinessException("WMS:BarcodeLabel:RuleInactive",
                $"Barcode rule '{rule.RuleName}' is inactive and cannot be used for generation.");

        var generatedCode = rule.GenerateNextCode();

        // Append reference ID if provided
        if (!string.IsNullOrWhiteSpace(referenceId))
        {
            generatedCode = $"{generatedCode}-{referenceId}";
        }

        await _barcodeRuleRepository.UpdateAsync(rule);

        return (rule, generatedCode);
    }

    /// <summary>
    /// Parse a barcode string and attempt to reverse-lookup the source information.
    /// Identifies the barcode type and format from the pattern.
    /// </summary>
    public async Task<(BarcodeType? BarcodeType, BarcodeFormat? BarcodeFormat, string? RawCode)> ParseBarcodeAsync(string barcodeString)
    {
        if (string.IsNullOrWhiteSpace(barcodeString))
            throw new BusinessException("WMS:BarcodeLabel:InvalidBarcode",
                "Barcode string cannot be empty.");

        var allRules = await _barcodeRuleRepository.GetActiveRulesAsync();

        // Try to match against active rules by checking if the barcode starts with a known prefix
        foreach (var rule in allRules)
        {
            if (!string.IsNullOrEmpty(rule.Prefix) && barcodeString.StartsWith(rule.Prefix))
            {
                return (rule.BarcodeType, rule.BarcodeFormat, barcodeString);
            }
        }

        // If no prefix matches, try to guess from regex patterns
        foreach (var rule in allRules)
        {
            try
            {
                // Replace pattern placeholders with regex groups
                var regexPattern = "^" + Regex.Escape(rule.CodePattern)
                    .Replace("\\{PREFIX\\}", "(?<prefix>[A-Z]+)")
                    .Replace("\\{SEQ\\}", "(?<seq>[0-9]+)")
                    .Replace("\\{DATE:yyyyMMdd\\}", "(?<date>[0-9]{8})")
                    .Replace("\\{DATE:yyMMddHHmmss\\}", "(?<datetime>[0-9]{12})")
                    .Replace("\\{TYPE\\}", "(?<type>[A-Za-z]+)")
                    + "(-(?<ref>.+))?$";

                if (Regex.IsMatch(barcodeString, regexPattern))
                {
                    return (rule.BarcodeType, rule.BarcodeFormat, barcodeString);
                }
            }
            catch
            {
                // Skip invalid regex patterns
            }
        }

        // Unable to identify — return raw code with null type info
        return (null, null, barcodeString);
    }

    /// <summary>
    /// Create a print task with auto-generated TaskNo.
    /// </summary>
    public async Task<PrintTask> CreatePrintTaskAsync(
        string sourceOrderType,
        Guid sourceOrderId,
        Guid templateId,
        string printContent,
        int printQuantity,
        string? printerId = null,
        string? printerName = null)
    {
        var template = await _labelTemplateRepository.GetAsync(templateId);

        if (!template.IsActive)
            throw new BusinessException("WMS:BarcodeLabel:TemplateInactive",
                $"Label template '{template.TemplateName}' is inactive and cannot be used for printing.");

        var taskNo = IdGenerator.NewOrderNo("PRT");

        var printTask = new PrintTask(
            GuidGenerator.Create(),
            taskNo,
            sourceOrderType,
            sourceOrderId,
            templateId,
            printContent,
            printQuantity,
            template.TemplateName,
            printerId,
            printerName);

        await _printTaskRepository.InsertAsync(printTask);

        return printTask;
    }
}
