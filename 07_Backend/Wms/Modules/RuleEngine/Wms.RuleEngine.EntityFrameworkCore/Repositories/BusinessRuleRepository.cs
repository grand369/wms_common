using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;
using Wms.RuleEngine.Domain.Repositories;

namespace Wms.RuleEngine.EntityFrameworkCore.Repositories;

/// <summary>
/// BusinessRuleRepository — implements IBusinessRuleRepository (REP-22).
/// </summary>
public class BusinessRuleRepository : EfCoreRepository<WmsRuleEngineDbContext, BusinessRule, Guid>,
    IBusinessRuleRepository
{
    public BusinessRuleRepository(IDbContextProvider<WmsRuleEngineDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<BusinessRule?> FindByRuleNameAsync(string ruleName)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(r => r.RuleName == ruleName);
    }

    public async Task<List<BusinessRule>> GetByRuleTypeAsync(RuleType ruleType)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(r => r.RuleType == ruleType)
            .OrderByDescending(r => r.CreationTime)
            .ToListAsync();
    }

    public async Task<BusinessRule?> GetByVersionAsync(string ruleName, int version)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(r => r.RuleName == ruleName && r.RuleVersion == version);
    }

    public async Task<List<BusinessRule>> GetEffectiveRulesAsync(RuleType ruleType)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet.Where(r =>
            r.RuleType == ruleType &&
            r.EffectiveStatus == EffectiveStatus.Active &&
            (r.EffectiveFrom == null || now >= r.EffectiveFrom) &&
            (r.EffectiveTo == null || now <= r.EffectiveTo))
            .OrderByDescending(r => r.RuleVersion)
            .ToListAsync();
    }
}
