using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Wms.BarcodeLabel.Application.Contracts.Dtos;
using Wms.BarcodeLabel.Application.Contracts.Permissions;
using Wms.BarcodeLabel.Application.Contracts.Services;
using Wms.BarcodeLabel.Application.Mappings;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Wms.BarcodeLabel.Domain.Repositories;
using Wms.BarcodeLabel.Domain.Services;
using Volo.Abp.Authorization;

namespace Wms.BarcodeLabel.Application.Services;

/// <summary>
/// BarcodeLabelAppService — application service implementing all 11 methods.
/// API-BL-001~011
/// </summary>
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IBarcodeRuleAppService))]
public class BarcodeLabelAppService : ApplicationService, IBarcodeRuleAppService
{
    private readonly IBarcodeRuleRepository _barcodeRuleRepository;
    private readonly ILabelTemplateRepository _labelTemplateRepository;
    private readonly IPrintTaskRepository _printTaskRepository;
    private readonly BarcodeLabelDomainService _barcodeLabelDomainService;

    static BarcodeLabelAppService()
    {
        BarcodeLabelAutoMapperProfile.Configure();
    }

    public BarcodeLabelAppService(
        IBarcodeRuleRepository barcodeRuleRepository,
        ILabelTemplateRepository labelTemplateRepository,
        IPrintTaskRepository printTaskRepository,
        BarcodeLabelDomainService barcodeLabelDomainService)
    {
        _barcodeRuleRepository = barcodeRuleRepository;
        _labelTemplateRepository = labelTemplateRepository;
        _printTaskRepository = printTaskRepository;
        _barcodeLabelDomainService = barcodeLabelDomainService;
    }

    // ── API-BL-001: GetListAsync ────────────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Read)]
    public async Task<PagedResultDto<BarcodeRuleOutputDto>> GetListAsync(BarcodeRuleQueryDto query)
    {
        var queryable = await _barcodeRuleRepository.GetQueryableAsync();

        if (query.BarcodeTypeValue.HasValue)
        {
            var barcodeType = BarcodeType.FromValue(query.BarcodeTypeValue.Value);
            queryable = queryable.Where(r => r.BarcodeType == barcodeType);
        }
        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(r => r.IsActive == query.IsActive.Value);
        }
        if (!string.IsNullOrEmpty(query.RuleName))
        {
            queryable = queryable.Where(r => r.RuleName.Contains(query.RuleName));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(r => r.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<BarcodeRuleOutputDto>(totalCount,
            items.Select(MapToBarcodeRuleOutputDto).ToList());
    }

    // ── API-BL-002: GetAsync ────────────────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Read)]
    public async Task<BarcodeRuleOutputDto> GetAsync(Guid id)
    {
        var rule = await _barcodeRuleRepository.GetAsync(id);
        return MapToBarcodeRuleOutputDto(rule);
    }

    // ── API-BL-003: CreateAsync ─────────────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Create)]
    public async Task<BarcodeRuleOutputDto> CreateAsync(BarcodeRuleCreateDto input)
    {
        var barcodeType = BarcodeType.FromValue(input.BarcodeTypeValue);
        var barcodeFormat = BarcodeFormat.FromValue(input.BarcodeFormatValue);

        var rule = new BarcodeRule(
            GuidGenerator.Create(),
            input.RuleName,
            barcodeType,
            barcodeFormat,
            input.CodePattern,
            input.Prefix,
            input.Description);

        await _barcodeRuleRepository.InsertAsync(rule);

        return MapToBarcodeRuleOutputDto(rule);
    }

    // ── API-BL-004: GetTemplateListAsync ────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Read)]
    public async Task<PagedResultDto<LabelTemplateOutputDto>> GetTemplateListAsync(LabelTemplateQueryDto query)
    {
        var queryable = await _labelTemplateRepository.GetQueryableAsync();

        if (query.TemplateTypeValue.HasValue)
        {
            var templateType = LabelTemplateType.FromValue(query.TemplateTypeValue.Value);
            queryable = queryable.Where(t => t.TemplateType == templateType);
        }
        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(t => t.IsActive == query.IsActive.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(t => t.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<LabelTemplateOutputDto>(totalCount,
            items.Select(MapToLabelTemplateOutputDto).ToList());
    }

    // ── API-BL-005: GetTemplateAsync ────────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Read)]
    public async Task<LabelTemplateOutputDto> GetTemplateAsync(Guid id)
    {
        var template = await _labelTemplateRepository.GetAsync(id);
        return MapToLabelTemplateOutputDto(template);
    }

    // ── API-BL-006: CreateTemplateAsync ─────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Create)]
    public async Task<LabelTemplateOutputDto> CreateTemplateAsync(LabelTemplateCreateDto input)
    {
        var templateType = LabelTemplateType.FromValue(input.TemplateTypeValue);

        var template = new LabelTemplate(
            GuidGenerator.Create(),
            input.TemplateName,
            templateType,
            input.TemplateContent,
            input.IndustryStandard);

        await _labelTemplateRepository.InsertAsync(template);

        return MapToLabelTemplateOutputDto(template);
    }

    // ── API-BL-007: GenerateBarcodeAsync ────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Generate)]
    public async Task<BarcodeResultDto> GenerateBarcodeAsync(BarcodeGenerateDto input)
    {
        var (rule, generatedCode) = await _barcodeLabelDomainService.GenerateBarcodeAsync(
            input.RuleId, input.ReferenceId ?? string.Empty);

        return new BarcodeResultDto
        {
            GeneratedCode = generatedCode,
            BarcodeTypeValue = rule.BarcodeType.Value,
            RuleId = rule.Id,
            BarcodeFormatValue = rule.BarcodeFormat.Value
        };
    }

    // ── API-BL-008: ParseBarcodeAsync ───────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Read)]
    public async Task<BarcodeResultDto> ParseBarcodeAsync(BarcodeParseDto input)
    {
        var (barcodeType, barcodeFormat, rawCode) = await _barcodeLabelDomainService.ParseBarcodeAsync(input.BarcodeString);

        return new BarcodeResultDto
        {
            GeneratedCode = rawCode,
            BarcodeTypeValue = barcodeType?.Value,
            RuleId = null, // Parsing does not always resolve a specific rule
            BarcodeFormatValue = barcodeFormat?.Value
        };
    }

    // ── API-BL-009: CreatePrintTaskAsync ────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Print)]
    public async Task<PrintTaskOutputDto> CreatePrintTaskAsync(PrintTaskCreateDto input)
    {
        var printTask = await _barcodeLabelDomainService.CreatePrintTaskAsync(
            input.SourceOrderType,
            input.SourceOrderId,
            input.TemplateId,
            input.PrintContent,
            input.PrintQuantity,
            input.PrinterId,
            input.PrinterName);

        return MapToPrintTaskOutputDto(printTask);
    }

    // ── API-BL-010: GetPrintTaskAsync ───────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Read)]
    public async Task<PrintTaskOutputDto> GetPrintTaskAsync(Guid id)
    {
        var task = await _printTaskRepository.GetAsync(id);
        return MapToPrintTaskOutputDto(task);
    }

    // ── API-BL-011: RetryPrintAsync ─────────────────────────

    [Authorize(WmsBarcodeLabelPermissions.Print)]
    public async Task<PrintTaskOutputDto> RetryPrintAsync(Guid id)
    {
        var task = await _printTaskRepository.GetAsync(id);
        task.Retry();
        await _printTaskRepository.UpdateAsync(task);
        return MapToPrintTaskOutputDto(task);
    }

    // ── Mapping Helpers ─────────────────────────────────────

    private BarcodeRuleOutputDto MapToBarcodeRuleOutputDto(BarcodeRule rule)
    {
        return rule.Adapt<BarcodeRuleOutputDto>();
    }

    private LabelTemplateOutputDto MapToLabelTemplateOutputDto(LabelTemplate template)
    {
        return template.Adapt<LabelTemplateOutputDto>();
    }

    private PrintTaskOutputDto MapToPrintTaskOutputDto(PrintTask task)
    {
        return task.Adapt<PrintTaskOutputDto>();
    }
}
