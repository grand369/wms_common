using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Volo.Abp.EntityFrameworkCore;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.EntityFrameworkCore.Configurations;

namespace Wms.BarcodeLabel.EntityFrameworkCore;

public class WmsBarcodeLabelDbContext : AbpDbContext<WmsBarcodeLabelDbContext>
{
    public DbSet<BarcodeRule> BarcodeRules { get; set; }
    public DbSet<LabelTemplate> LabelTemplates { get; set; }
    public DbSet<PrintTask> PrintTasks { get; set; }

    public WmsBarcodeLabelDbContext(DbContextOptions<WmsBarcodeLabelDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new BarcodeRuleConfiguration());
        builder.ApplyConfiguration(new LabelTemplateConfiguration());
        builder.ApplyConfiguration(new PrintTaskConfiguration());
    }
}
