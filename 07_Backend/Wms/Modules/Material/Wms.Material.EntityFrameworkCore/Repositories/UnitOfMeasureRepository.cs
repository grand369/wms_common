using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Material.Domain.Entities;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.EntityFrameworkCore.Repositories;

public class UnitOfMeasureRepository : EfCoreRepository<WmsMaterialDbContext, UnitOfMeasure, Guid>, IUnitOfMeasureRepository
{
    public UnitOfMeasureRepository(IDbContextProvider<WmsMaterialDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<UnitOfMeasure?> FindByCodeAsync(string unitCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.UnitOfMeasures.FirstOrDefaultAsync(u => u.UnitCode == unitCode);
    }

    public async Task<List<UnitOfMeasure>> GetActiveListAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.UnitOfMeasures.Where(u => u.IsActive).OrderBy(u => u.UnitCode).ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string unitCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.UnitOfMeasures.AnyAsync(u => u.UnitCode == unitCode);
    }
}
