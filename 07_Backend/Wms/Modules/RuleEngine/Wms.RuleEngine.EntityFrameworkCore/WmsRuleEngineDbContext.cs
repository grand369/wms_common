using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.EntityFrameworkCore.Configurations;

namespace Wms.RuleEngine.EntityFrameworkCore;

public class WmsRuleEngineDbContext : AbpDbContext<WmsRuleEngineDbContext>
{
    public DbSet<BusinessRule> BusinessRules { get; set; }
    public DbSet<IndustryPackage> IndustryPackages { get; set; }

    public WmsRuleEngineDbContext(DbContextOptions<WmsRuleEngineDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureRuleEngine(builder);
    }

    private void ConfigureRuleEngine(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BusinessRuleConfiguration());
        builder.ApplyConfiguration(new IndustryPackageConfiguration());
    }
}
