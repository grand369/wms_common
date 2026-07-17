using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Material.Domain.Aggregates;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.EntityFrameworkCore.Repositories;

public class MaterialIssueStrategyRepository : EfCoreRepository<WmsMaterialDbContext, MaterialIssueStrategy, Guid>, IMaterialIssueStrategyRepository
{
    public MaterialIssueStrategyRepository(IDbContextProvider<WmsMaterialDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<bool> CodeExistsAsync(string code)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.MaterialIssueStrategies.AnyAsync(s => s.Code == code);
    }

    public async Task<MaterialIssueStrategy?> FindByCodeAsync(string code)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.MaterialIssueStrategies.FirstOrDefaultAsync(s => s.Code == code);
    }
}
