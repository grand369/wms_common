using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Wms.BarcodeLabel.Domain.Repositories;

namespace Wms.BarcodeLabel.EntityFrameworkCore.Repositories;

public class BarcodeRuleRepository : EfCoreRepository<WmsBarcodeLabelDbContext, BarcodeRule, Guid>, IBarcodeRuleRepository
{
    public BarcodeRuleRepository(IDbContextProvider<WmsBarcodeLabelDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<BarcodeRule?> FindByRuleNameAsync(string ruleName)
        => await (await GetDbSetAsync()).FirstOrDefaultAsync(r => r.RuleName == ruleName);

    public async Task<List<BarcodeRule>> GetByBarcodeTypeAsync(BarcodeType barcodeType)
        => await (await GetDbSetAsync()).Where(r => r.BarcodeType == barcodeType).OrderBy(r => r.RuleName).ToListAsync();

    public async Task<List<BarcodeRule>> GetActiveRulesAsync()
        => await (await GetDbSetAsync()).Where(r => r.IsActive).OrderBy(r => r.RuleName).ToListAsync();
}
