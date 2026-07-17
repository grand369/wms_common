using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.LineSide.Domain.Aggregates;
using Wms.Shared.Domain.Interfaces;

namespace Wms.LineSide.Domain.Repositories;

/// <summary>REP-14: LineSideWarehouse repository</summary>
public interface ILineSideWarehouseRepository : IBasicRepository<LineSideWarehouse, Guid>
{
    Task<LineSideWarehouse?> FindByCodeAsync(string code);
    Task<List<LineSideWarehouse>> GetByProductionLineAsync(Guid productionLineId);
    Task<List<LineSideWarehouse>> GetBelowMinAsync();
    Task<LineSideWarehouse> GetWithKanbanItemsAsync(Guid id);
}
