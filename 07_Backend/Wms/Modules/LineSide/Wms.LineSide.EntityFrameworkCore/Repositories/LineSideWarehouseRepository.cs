using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.LineSide.Domain.Aggregates;
using Wms.LineSide.Domain.Repositories;

namespace Wms.LineSide.EntityFrameworkCore.Repositories;

public class LineSideWarehouseRepository : EfCoreRepository<WmsLineSideDbContext, LineSideWarehouse, Guid>, ILineSideWarehouseRepository
{
    public LineSideWarehouseRepository(IDbContextProvider<WmsLineSideDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<LineSideWarehouse?> FindByCodeAsync(string code)
        => await (await GetDbSetAsync()).FirstOrDefaultAsync(w => w.LineSideWarehouseCode == code);

    public async Task<List<LineSideWarehouse>> GetByProductionLineAsync(Guid productionLineId)
        => await (await GetDbSetAsync()).Where(w => w.ProductionLineId == productionLineId).ToListAsync();

    public async Task<List<LineSideWarehouse>> GetBelowMinAsync()
        => await (await GetDbSetAsync()).Include(w => w.KanbanItems)
            .Where(w => w.KanbanItems.Any(k => k.CurrentQuantity < k.MinQuantity)).ToListAsync();

    public async Task<LineSideWarehouse> GetWithKanbanItemsAsync(Guid id)
        => await (await GetDbSetAsync()).Include(w => w.KanbanItems).FirstOrDefaultAsync(w => w.Id == id)
            ?? throw new EntityNotFoundException(typeof(LineSideWarehouse), id);
}
