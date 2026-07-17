using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.CycleCount.Domain.Aggregates;
using Wms.CycleCount.Domain.Enums;

namespace Wms.CycleCount.Domain.Repositories;

/// <summary>REP-12: CycleCountPlan repository</summary>
public interface ICycleCountPlanRepository : IBasicRepository<CycleCountPlan, Guid>
{
    Task<CycleCountPlan?> FindByNoAsync(string planNo);
    Task<List<CycleCountPlan>> GetByStatusAsync(CountStatus status);
    Task<List<CycleCountPlan>> GetByWarehouseAsync(Guid warehouseId);
    Task<CycleCountPlan> GetWithItemsAsync(Guid id);
}

/// <summary>REP-13: CycleCountResult repository</summary>
public interface ICycleCountResultRepository : IBasicRepository<CycleCountResult, Guid>
{
    Task<List<CycleCountResult>> GetByPlanIdAsync(Guid planId);
    Task<List<CycleCountResult>> GetDifferencesOverThresholdAsync(decimal thresholdPercent);
}
