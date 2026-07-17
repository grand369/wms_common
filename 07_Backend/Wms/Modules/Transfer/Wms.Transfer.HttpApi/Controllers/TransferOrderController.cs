using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Transfer.Application.Contracts.Dtos;
using Wms.Transfer.Application.Contracts.Services;

namespace Wms.Transfer.HttpApi.Controllers;

/// <summary>
/// TransferOrderController – REST API endpoints API-TF-001~010
/// Base route: /api/v1/transfer/orders
/// </summary>
[RemoteService(Name = "WmsTransfer")]
[Area("WmsTransfer")]
[Route("api/v1/transfer/orders")]
[Authorize]
public class TransferOrderController : AbpControllerBase
{
    private readonly ITransferOrderAppService _appService;

    public TransferOrderController(ITransferOrderAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<TransferOrderOutputDto>> GetListAsync(TransferOrderQueryDto query)
        => _appService.GetListAsync(query);

    [HttpGet("{id}")]
    public Task<TransferOrderOutputDto> GetAsync(Guid id)
        => _appService.GetAsync(id);

    [HttpPost]
    public Task<TransferOrderOutputDto> CreateAsync(TransferOrderCreateDto input)
        => _appService.CreateAsync(input);

    [HttpPut("{id}")]
    public Task<TransferOrderOutputDto> UpdateAsync(Guid id, TransferOrderUpdateDto input)
        => _appService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
        => _appService.DeleteAsync(id);

    [HttpPatch("{id}/submit-approval")]
    public Task<TransferOrderOutputDto> SubmitApprovalAsync(Guid id)
        => _appService.SubmitApprovalAsync(id);

    [HttpPatch("{id}/approve")]
    public Task<TransferOrderOutputDto> ApproveAsync(Guid id)
        => _appService.ApproveAsync(id);

    [HttpPatch("{id}/outbound-confirm")]
    public Task<TransferOrderOutputDto> ConfirmOutboundAsync(Guid id, [FromBody] ConfirmTransferOutboundCommandDto input)
        => _appService.ConfirmOutboundAsync(id, input);

    [HttpPatch("{id}/inbound-confirm")]
    public Task<TransferOrderOutputDto> ConfirmInboundAsync(Guid id, [FromBody] ConfirmTransferInboundCommandDto input)
        => _appService.ConfirmInboundAsync(id, input);

    [HttpPatch("{id}/complete")]
    public Task<TransferOrderOutputDto> CompleteAsync(Guid id)
        => _appService.CompleteAsync(id);
}
