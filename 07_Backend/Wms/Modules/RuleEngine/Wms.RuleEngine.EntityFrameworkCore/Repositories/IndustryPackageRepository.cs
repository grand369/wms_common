using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;
using Wms.RuleEngine.Domain.Repositories;

namespace Wms.RuleEngine.EntityFrameworkCore.Repositories;

/// <summary>
/// IndustryPackageRepository — implements IIndustryPackageRepository (REP-23).
/// </summary>
public class IndustryPackageRepository : EfCoreRepository<WmsRuleEngineDbContext, IndustryPackage, Guid>,
    IIndustryPackageRepository
{
    public IndustryPackageRepository(IDbContextProvider<WmsRuleEngineDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<IndustryPackage?> FindByPackageNameAsync(string packageName)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(p => p.PackageName == packageName);
    }

    public async Task<List<IndustryPackage>> GetByIndustryTypeAsync(IndustryType industryType)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(p => p.IndustryType == industryType)
            .OrderByDescending(p => p.CreationTime)
            .ToListAsync();
    }
}
