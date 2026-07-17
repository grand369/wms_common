using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.EntityFrameworkCore.Configurations;

namespace Wms.Inbound.EntityFrameworkCore;

/// <summary>
/// Inbound Module DbContext — registers all Inbound module entities and configurations.
/// </summary>
public class WmsInboundDbContext : AbpDbContext<WmsInboundDbContext>
{
    // BC-04 Inbound — Core entities
    public DbSet<InboundOrder> InboundOrders { get; set; }
    public DbSet<InboundLine> InboundLines { get; set; }

    public WmsInboundDbContext(DbContextOptions<WmsInboundDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all EF Core configurations for Inbound module
        builder.ApplyConfiguration(new InboundOrderConfiguration());
        builder.ApplyConfiguration(new InboundLineConfiguration());
    }
}
