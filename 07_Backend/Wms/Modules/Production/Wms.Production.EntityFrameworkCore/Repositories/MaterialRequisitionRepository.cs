using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Production.Domain.Aggregates;
using Wms.Production.Domain.Repositories;

namespace Wms.Production.EntityFrameworkCore.Repositories;

public class MaterialRequisitionRepository : EfCoreRepository<WmsProductionDbContext, MaterialRequisition, Guid>, IMaterialRequisitionRepository
{
    public MaterialRequisitionRepository(IDbContextProvider<WmsProductionDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<MaterialRequisition?> FindByNoAsync(string requisitionNo)
        => await (await GetDbSetAsync()).FirstOrDefaultAsync(r => r.RequisitionNo == requisitionNo);

    public async Task<List<MaterialRequisition>> GetByProductionOrderAsync(Guid productionOrderId)
        => await (await GetDbSetAsync()).Where(r => r.ProductionOrderId == productionOrderId).ToListAsync();

    public async Task<MaterialRequisition> GetWithLinesAsync(Guid id)
        => await (await GetDbSetAsync()).Include(r => r.Lines).FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new EntityNotFoundException(typeof(MaterialRequisition), id);
}
