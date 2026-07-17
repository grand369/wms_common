using Microsoft.EntityFrameworkCore;
using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using Wms.Inbound.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inbound.EntityFrameworkCore.Repositories;

/// <summary>
/// InboundOrder Repository Implementation — implements all custom query methods from REP-08.
/// </summary>
public class InboundOrderRepository : EfCoreRepository<WmsInboundDbContext, InboundOrder, Guid>,
    IInboundOrderRepository
{
    public InboundOrderRepository(IDbContextProvider<WmsInboundDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<InboundOrder?> FindByNoAsync(string inboundOrderNo)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(o => o.InboundOrderNo == inboundOrderNo);
    }

    public async Task<List<InboundOrder>> GetListByWarehouseAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o => o.WarehouseId == warehouseId)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<InboundOrder>> GetListByTypeAsync(InboundType inboundType)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o => o.InboundType == inboundType)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<InboundOrder>> GetListByStatusAsync(InboundStatus status)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o => o.InboundStatus == status)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<InboundOrder>> GetPendingInspectionAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o =>
            o.WarehouseId == warehouseId &&
            o.InboundStatus == InboundStatus.Inspecting)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }

    public async Task<List<InboundOrder>> GetPendingPutawayAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(o =>
            o.WarehouseId == warehouseId &&
            o.InboundStatus == InboundStatus.Putaway)
            .OrderByDescending(o => o.CreationTime).ToListAsync();
    }
}
