using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.CycleCount.Domain.Aggregates;
using Wms.CycleCount.Domain.Enums;
using Wms.CycleCount.Domain.Repositories;

namespace Wms.CycleCount.EntityFrameworkCore.Repositories;

public class CycleCountPlanRepository : EfCoreRepository<WmsCycleCountDbContext, CycleCountPlan, Guid>, ICycleCountPlanRepository
{
    public CycleCountPlanRepository(IDbContextProvider<WmsCycleCountDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<CycleCountPlan?> FindByNoAsync(string planNo)
        => await (await GetDbSetAsync()).FirstOrDefaultAsync(p => p.PlanNo == planNo);

    public async Task<List<CycleCountPlan>> GetByStatusAsync(CountStatus status)
        => await (await GetDbSetAsync()).Where(p => p.CountStatus == status).ToListAsync();

    public async Task<List<CycleCountPlan>> GetByWarehouseAsync(Guid warehouseId)
        => await (await GetDbSetAsync()).Where(p => p.WarehouseId == warehouseId).ToListAsync();

    public async Task<CycleCountPlan> GetWithItemsAsync(Guid id)
        => await (await GetDbSetAsync()).Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new EntityNotFoundException(typeof(CycleCountPlan), id);
}

public class CycleCountResultRepository : EfCoreRepository<WmsCycleCountDbContext, CycleCountResult, Guid>, ICycleCountResultRepository
{
    public CycleCountResultRepository(IDbContextProvider<WmsCycleCountDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<List<CycleCountResult>> GetByPlanIdAsync(Guid planId)
        => await (await GetDbSetAsync()).Where(r => r.PlanId == planId).ToListAsync();

    public async Task<List<CycleCountResult>> GetDifferencesOverThresholdAsync(decimal thresholdPercent)
        => await (await GetDbSetAsync()).Where(r => Math.Abs(r.DifferenceQuantity) > 0).ToListAsync();
}
