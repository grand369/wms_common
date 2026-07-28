using Wms.Inbound.Application.Contracts.Dtos;
using Volo.Abp.Application.Dtos;

namespace Wms.Inbound.Application.Contracts.Services;

/// <summary>
/// IInboundOrderAppService — application service interface for inbound order operations.
/// Covers CRUD + domain-specific operations (Confirm, QualityInspect, Putaway, Complete, Cancel).
/// (API-IN-001~013)
/// </summary>
public interface IInboundOrderAppService
{
    /// <summary>API-IN-001: Get inbound order list with paging and filtering.</summary>
    Task<PagedResultDto<InboundOrderOutputDto>> GetListAsync(InboundOrderQueryDto query);

    /// <summary>API-IN-002: Get inbound order detail.</summary>
    Task<InboundOrderOutputDto> GetAsync(Guid id);

    /// <summary>API-IN-003: Create inbound order.</summary>
    Task<InboundOrderOutputDto> CreateAsync(InboundOrderCreateDto dto);

    /// <summary>API-IN-004: Update inbound order (only in Draft status).</summary>
    Task<InboundOrderOutputDto> UpdateAsync(Guid id, InboundOrderUpdateDto dto);

    /// <summary>API-IN-005: Delete inbound order (only in Draft status).</summary>
    Task DeleteAsync(Guid id);

    /// <summary>API-IN-006: Confirm receipt — record received quantities.</summary>
    Task<InboundOrderOutputDto> ConfirmAsync(Guid id, InboundConfirmCommandDto dto);

    /// <summary>API-IN-007: Quality inspection — process quality results for lines.</summary>
    Task<InboundOrderOutputDto> QualityInspectAsync(Guid id, InboundQualityInspectCommandDto dto);

    /// <summary>API-IN-008: Putaway — confirm putaway locations for lines.</summary>
    Task<InboundOrderOutputDto> PutawayAsync(Guid id, InboundPutawayCommandDto dto);

    /// <summary>API-IN-009: Complete inbound order — increase inventory synchronously.</summary>
    Task<InboundOrderOutputDto> CompleteAsync(Guid id);

    /// <summary>API-IN-010: Cancel inbound order.</summary>
    Task<InboundOrderOutputDto> CancelAsync(Guid id);

    /// <summary>API-IN-011: Recommend putaway locations for a line.</summary>
    Task<List<InboundRecommendLocationResultDto>> RecommendPutawayLocationsAsync(Guid id, Guid lineId);

    /// <summary>API-IN-012: Batch create inbound orders.</summary>
    Task<List<InboundOrderOutputDto>> BatchCreateAsync(List<InboundOrderCreateDto> dtos);

    /// <summary>API-IN-013: Get inbound order by order number.</summary>
    Task<InboundOrderOutputDto> GetByNoAsync(string orderNo);

    /// <summary>API-IN-014: Get inbound order statistics.</summary>
    Task<InboundStatisticsDto> GetStatisticsAsync(InboundStatisticsQueryDto query);

    /// <summary>API-IN-015: ERP callback for inbound order.</summary>
    Task<InboundOrderOutputDto> ErpCallbackAsync(Guid id, InboundErpCallbackDto dto);
}
