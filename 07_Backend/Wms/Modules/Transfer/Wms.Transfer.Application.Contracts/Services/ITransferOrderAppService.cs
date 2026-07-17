using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Wms.Transfer.Application.Contracts.Dtos;

namespace Wms.Transfer.Application.Contracts.Services;

/// <summary>
/// ITransferOrderAppService — 10 API methods (5 CRUD + 5 business operations)
/// API-TF-001~010
/// </summary>
public interface ITransferOrderAppService : IApplicationService
{
    // ── CRUD ──────────────────────────────────────────────
    Task<PagedResultDto<TransferOrderOutputDto>> GetListAsync(TransferOrderQueryDto query);
    Task<TransferOrderOutputDto> GetAsync(Guid id);
    Task<TransferOrderOutputDto> CreateAsync(TransferOrderCreateDto input);
    Task<TransferOrderOutputDto> UpdateAsync(Guid id, TransferOrderUpdateDto input);
    Task DeleteAsync(Guid id);

    // ── Business Operations ───────────────────────────────
    Task<TransferOrderOutputDto> SubmitApprovalAsync(Guid id);
    Task<TransferOrderOutputDto> ApproveAsync(Guid id);
    Task<TransferOrderOutputDto> ConfirmOutboundAsync(Guid id, ConfirmTransferOutboundCommandDto input);
    Task<TransferOrderOutputDto> ConfirmInboundAsync(Guid id, ConfirmTransferInboundCommandDto input);
    Task<TransferOrderOutputDto> CompleteAsync(Guid id);
}
