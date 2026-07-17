using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Outbound.Domain.Aggregates;

namespace Wms.Outbound.EntityFrameworkCore;

/// <summary>
/// Outbound DbContext — registers OutboundOrder and OutboundLine DbSets.
/// Configurations are applied via IEntityTypeConfiguration in Configurations folder.
/// </summary>
public class WmsOutboundDbContext : AbpDbContext<WmsOutboundDbContext>
{
    public DbSet<OutboundOrder> OutboundOrders { get; set; } = null!;
    public DbSet<OutboundLine> OutboundLines { get; set; } = null!;

    public WmsOutboundDbContext(DbContextOptions<WmsOutboundDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new Configurations.OutboundOrderConfiguration());
        builder.ApplyConfiguration(new Configurations.OutboundLineConfiguration());
    }
}
