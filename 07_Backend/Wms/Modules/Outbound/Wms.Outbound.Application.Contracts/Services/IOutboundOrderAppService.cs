using Wms.Outbound.Application.Contracts.Dtos;
using Volo.Abp.Application.Dtos;

namespace Wms.Outbound.Application.Contracts.Services;

/// <summary>
/// IOutboundOrderAppService — application service interface for outbound order operations.
/// Covers CRUD + domain-specific operations (Allocate, Picking, Shipping, Complete, Cancel, ReleaseAllocation).
/// (API-OB-001~012)
/// </summary>
public interface IOutboundOrderAppService
{
    /// <summary>API-OB-001: Get outbound order list with paging and filtering.</summary>
    Task<PagedResultDto<OutboundOrderOutputDto>> GetListAsync(OutboundOrderQueryDto query);

    /// <summary>API-OB-002: Get outbound order detail.</summary>
    Task<OutboundOrderOutputDto> GetAsync(Guid id);

    /// <summary>API-OB-003: Create outbound order.</summary>
    Task<OutboundOrderOutputDto> CreateAsync(OutboundOrderCreateDto dto);

    /// <summary>API-OB-004: Update outbound order (only in Draft status).</summary>
    Task<OutboundOrderOutputDto> UpdateAsync(Guid id, OutboundOrderUpdateDto dto);

    /// <summary>API-OB-005: Delete outbound order (only in Draft status).</summary>
    Task DeleteAsync(Guid id);

    /// <summary>API-OB-006: Allocate inventory — reserve stock for outbound lines.</summary>
    Task<OutboundOrderOutputDto> AllocateAsync(Guid id, OutboundAllocateCommandDto dto);

    /// <summary>API-OB-007: Confirm picking — record picked quantities.</summary>
    Task<OutboundOrderOutputDto> PickingAsync(Guid id, OutboundPickingCommandDto dto);

    /// <summary>API-OB-008: Confirm shipping — record shipped quantities.</summary>
    Task<OutboundOrderOutputDto> ShippingAsync(Guid id, OutboundShippingCommandDto dto);

    /// <summary>API-OB-009: Complete outbound order — decrease inventory synchronously.</summary>
    Task<OutboundOrderOutputDto> CompleteAsync(Guid id);

    /// <summary>API-OB-010: Cancel outbound order.</summary>
    Task<OutboundOrderOutputDto> CancelAsync(Guid id);

    /// <summary>API-OB-011: Release allocation — return to Draft and release reserved inventory.</summary>
    Task<OutboundOrderOutputDto> ReleaseAllocationAsync(Guid id);

    /// <summary>API-OB-012: Get outbound order by order number.</summary>
    Task<OutboundOrderOutputDto> GetByNoAsync(string orderNo);

    /// <summary>API-OB-013: ERP callback for outbound order.</summary>
    Task<OutboundOrderOutputDto> ErpCallbackAsync(Guid id, OutboundErpCallbackDto dto);

    /// <summary>API-OB-014: Get print data for outbound order.</summary>
    Task<OutboundOrderOutputDto> GetPrintDataAsync(Guid id, OutboundPrintDto dto);
}
