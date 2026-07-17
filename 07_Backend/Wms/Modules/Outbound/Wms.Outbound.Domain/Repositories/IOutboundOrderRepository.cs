using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Domain.Repositories;

namespace Wms.Outbound.Domain.Repositories;

/// <summary>
/// IOutboundOrderRepository (REP-09) — custom query methods for OutboundOrder aggregate.
/// Inherits IRepository<OutboundOrder, Guid> for standard CRUD.
/// </summary>
public interface IOutboundOrderRepository : IRepository<OutboundOrder, Guid>
{
    /// <summary>Find outbound order by order number (unique business key).</summary>
    Task<OutboundOrder?> FindByNoAsync(string outboundOrderNo);

    /// <summary>Get outbound orders by warehouse ID.</summary>
    Task<List<OutboundOrder>> GetListByWarehouseAsync(Guid warehouseId);

    /// <summary>Get outbound orders by outbound type.</summary>
    Task<List<OutboundOrder>> GetListByTypeAsync(OutboundType outboundType);

    /// <summary>Get emergency outbound orders for a warehouse.</summary>
    Task<List<OutboundOrder>> GetEmergencyOrdersAsync(Guid warehouseId);

    /// <summary>Get outbound orders pending allocation for a warehouse.</summary>
    Task<List<OutboundOrder>> GetPendingAllocationAsync(Guid warehouseId);
}
