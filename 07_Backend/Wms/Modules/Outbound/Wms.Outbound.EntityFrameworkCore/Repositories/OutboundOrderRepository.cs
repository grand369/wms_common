using Microsoft.EntityFrameworkCore;
using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;
using Wms.Outbound.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Outbound.EntityFrameworkCore.Repositories;

/// <summary>
/// OutboundOrder Repository Implementation — implements all custom query methods from REP-09.
/// </summary>
public class OutboundOrderRepository : EfCoreRepository<WmsOutboundDbContext, OutboundOrder, Guid>,
    IOutboundOrderRepository
{
    public OutboundOrderRepository(IDbContextProvider<WmsOutboundDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<OutboundOrder?> FindByNoAsync(string outboundOrderNo)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(o => o.OutboundOrderNo == outboundOrderNo);
    }

    public async Task<List<OutboundOrder>> GetListByWarehouseAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o => o.WarehouseId == warehouseId)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<OutboundOrder>> GetListByTypeAsync(OutboundType outboundType)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o => o.OutboundType == outboundType)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<OutboundOrder>> GetEmergencyOrdersAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o =>
            o.WarehouseId == warehouseId &&
            o.IsEmergency)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<OutboundOrder>> GetPendingAllocationAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o =>
            o.WarehouseId == warehouseId &&
            o.OutboundStatus == OutboundStatus.Draft)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }
}
