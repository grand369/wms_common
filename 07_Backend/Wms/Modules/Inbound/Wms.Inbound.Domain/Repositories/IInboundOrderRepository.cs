using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inbound.Domain.Repositories;

/// <summary>
/// IInboundOrderRepository (REP-08) — custom query methods for InboundOrder aggregate.
/// Inherits IRepository<InboundOrder, Guid> for standard CRUD.
/// </summary>
public interface IInboundOrderRepository : IRepository<InboundOrder, Guid>
{
    /// <summary>Find inbound order by order number (unique business key).</summary>
    Task<InboundOrder?> FindByNoAsync(string inboundOrderNo);

    /// <summary>Get inbound orders by warehouse ID.</summary>
    Task<List<InboundOrder>> GetListByWarehouseAsync(Guid warehouseId);

    /// <summary>Get inbound orders by inbound type.</summary>
    Task<List<InboundOrder>> GetListByTypeAsync(InboundType inboundType);

    /// <summary>Get inbound orders by status.</summary>
    Task<List<InboundOrder>> GetListByStatusAsync(InboundStatus status);

    /// <summary>Get inbound orders pending quality inspection in a warehouse.</summary>
    Task<List<InboundOrder>> GetPendingInspectionAsync(Guid warehouseId);

    /// <summary>Get inbound orders pending putaway in a warehouse.</summary>
    Task<List<InboundOrder>> GetPendingPutawayAsync(Guid warehouseId);
}
