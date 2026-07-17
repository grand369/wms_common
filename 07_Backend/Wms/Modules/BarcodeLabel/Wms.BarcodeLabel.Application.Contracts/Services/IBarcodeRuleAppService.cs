using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Wms.BarcodeLabel.Application.Contracts.Dtos;

namespace Wms.BarcodeLabel.Application.Contracts.Services;

/// <summary>
/// IBarcodeRuleAppService — application service for barcode label operations.
/// 11 methods: 3 BarcodeRule CRUD, 3 LabelTemplate CRUD, 2 Barcode operations, 3 PrintTask operations.
/// (API-BL-001~011)
/// </summary>
public interface IBarcodeRuleAppService : IApplicationService
{
    /// <summary>API-BL-001: Get paginated list of barcode rules.</summary>
    Task<PagedResultDto<BarcodeRuleOutputDto>> GetListAsync(BarcodeRuleQueryDto query);

    /// <summary>API-BL-002: Get barcode rule by ID.</summary>
    Task<BarcodeRuleOutputDto> GetAsync(Guid id);

    /// <summary>API-BL-003: Create a new barcode rule.</summary>
    Task<BarcodeRuleOutputDto> CreateAsync(BarcodeRuleCreateDto input);

    /// <summary>API-BL-004: Get paginated list of label templates.</summary>
    Task<PagedResultDto<LabelTemplateOutputDto>> GetTemplateListAsync(LabelTemplateQueryDto query);

    /// <summary>API-BL-005: Get label template by ID.</summary>
    Task<LabelTemplateOutputDto> GetTemplateAsync(Guid id);

    /// <summary>API-BL-006: Create a new label template.</summary>
    Task<LabelTemplateOutputDto> CreateTemplateAsync(LabelTemplateCreateDto input);

    /// <summary>API-BL-007: Generate barcode from rule and reference ID.</summary>
    Task<BarcodeResultDto> GenerateBarcodeAsync(BarcodeGenerateDto input);

    /// <summary>API-BL-008: Parse and reverse-lookup a barcode string.</summary>
    Task<BarcodeResultDto> ParseBarcodeAsync(BarcodeParseDto input);

    /// <summary>API-BL-009: Create a print task.</summary>
    Task<PrintTaskOutputDto> CreatePrintTaskAsync(PrintTaskCreateDto input);

    /// <summary>API-BL-010: Get print task by ID.</summary>
    Task<PrintTaskOutputDto> GetPrintTaskAsync(Guid id);

    /// <summary>API-BL-011: Retry a failed print task.</summary>
    Task<PrintTaskOutputDto> RetryPrintAsync(Guid id);
}
