using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Material.Domain.Aggregates;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;
using Wms.Material.Domain.Entities;
using Wms.Material.EntityFrameworkCore.Configurations;

namespace Wms.Material.EntityFrameworkCore;

/// <summary>
/// Material Module DbContext — v1.0 contributes tables to the shared WmsDbContext.
/// v2.0 can become an independent DbContext when modules are split into microservices.
/// Contains DbSet for Material, MaterialClassification, MaterialSubstituteRelation, and UnitOfMeasure entities.
/// (Phase 5 Database Design)
/// </summary>
public class WmsMaterialDbContext : AbpDbContext<WmsMaterialDbContext>
{
    // BC-02 Material DbSet declarations
    public DbSet<MaterialAgg> Materials { get; set; }
    public DbSet<MaterialClassification> MaterialClassifications { get; set; }
    public DbSet<MaterialSubstituteRelation> MaterialSubstituteRelations { get; set; }
    public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
    public DbSet<MaterialIssueStrategy> MaterialIssueStrategies { get; set; }

    public WmsMaterialDbContext(DbContextOptions<WmsMaterialDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all Material EF Core configurations
        builder.ApplyConfiguration(new MaterialConfiguration());
        builder.ApplyConfiguration(new MaterialClassificationConfiguration());
        builder.ApplyConfiguration(new MaterialSubstituteRelationConfiguration());
        builder.ApplyConfiguration(new UnitOfMeasureConfiguration());
        builder.ApplyConfiguration(new MaterialIssueStrategyConfiguration());
    }
}
