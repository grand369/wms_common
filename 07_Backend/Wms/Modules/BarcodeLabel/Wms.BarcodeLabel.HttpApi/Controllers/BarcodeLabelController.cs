using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Wms.BarcodeLabel.Application.Contracts.Dtos;
using Wms.BarcodeLabel.Application.Contracts.Services;

namespace Wms.BarcodeLabel.HttpApi.Controllers;

[RemoteService(Name = "WmsBarcodeLabel")]
[Area("WmsBarcodeLabel")]
[Route("api/v1/barcode-label")]
[Authorize]
public class BarcodeLabelController : AbpControllerBase
{
    private readonly IBarcodeRuleAppService _appService;

    public BarcodeLabelController(IBarcodeRuleAppService appService) => _appService = appService;

    [HttpGet("rules")]
    public Task<PagedResultDto<BarcodeRuleOutputDto>> GetListAsync(BarcodeRuleQueryDto query) => _appService.GetListAsync(query);

    [HttpGet("rules/{id}")]
    public Task<BarcodeRuleOutputDto> GetAsync(Guid id) => _appService.GetAsync(id);

    [HttpPost("rules")]
    public Task<BarcodeRuleOutputDto> CreateAsync(BarcodeRuleCreateDto input) => _appService.CreateAsync(input);

    [HttpGet("templates")]
    public Task<PagedResultDto<LabelTemplateOutputDto>> GetTemplateListAsync(LabelTemplateQueryDto query) => _appService.GetTemplateListAsync(query);

    [HttpGet("templates/{id}")]
    public Task<LabelTemplateOutputDto> GetTemplateAsync(Guid id) => _appService.GetTemplateAsync(id);

    [HttpPost("templates")]
    public Task<LabelTemplateOutputDto> CreateTemplateAsync(LabelTemplateCreateDto input) => _appService.CreateTemplateAsync(input);

    [HttpPost("barcode/generate")]
    public Task<BarcodeResultDto> GenerateBarcodeAsync(BarcodeGenerateDto input) => _appService.GenerateBarcodeAsync(input);

    [HttpPost("barcode/parse")]
    public Task<BarcodeResultDto> ParseBarcodeAsync(BarcodeParseDto input) => _appService.ParseBarcodeAsync(input);

    [HttpPost("print-jobs")]
    public Task<PrintTaskOutputDto> CreatePrintTaskAsync(PrintTaskCreateDto input) => _appService.CreatePrintTaskAsync(input);

    [HttpGet("print-jobs/{id}")]
    public Task<PrintTaskOutputDto> GetPrintTaskAsync(Guid id) => _appService.GetPrintTaskAsync(id);

    [HttpPatch("print-jobs/{id}/retry")]
    public Task<PrintTaskOutputDto> RetryPrintAsync(Guid id) => _appService.RetryPrintAsync(id);
}
