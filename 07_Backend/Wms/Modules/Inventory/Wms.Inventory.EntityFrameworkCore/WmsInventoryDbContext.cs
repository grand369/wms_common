using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.EntityFrameworkCore.Configurations;

namespace Wms.Inventory.EntityFrameworkCore;

/// <summary>
/// Inventory Module DbContext — registers all Inventory module entities and configurations.
/// </summary>
public class WmsInventoryDbContext : AbpDbContext<WmsInventoryDbContext>
{
    // BC-03 Inventory — Core entities
    public DbSet<InventoryBalance> InventoryBalances { get; set; }
    public DbSet<InventoryLedgerEntry> InventoryLedgerEntries { get; set; }
    public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }
    public DbSet<InventoryAdjustmentLine> InventoryAdjustmentLines { get; set; }
    public DbSet<InventoryFreezeOrder> InventoryFreezeOrders { get; set; }
    public DbSet<InventoryAlert> InventoryAlerts { get; set; }

    public WmsInventoryDbContext(DbContextOptions<WmsInventoryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all EF Core configurations for Inventory module
        builder.ApplyConfiguration(new InventoryBalanceConfiguration());
        builder.ApplyConfiguration(new InventoryLedgerEntryConfiguration());
        builder.ApplyConfiguration(new InventoryAdjustmentConfiguration());
        builder.ApplyConfiguration(new InventoryAdjustmentLineConfiguration());
        builder.ApplyConfiguration(new InventoryFreezeOrderConfiguration());
        builder.ApplyConfiguration(new InventoryAlertConfiguration());
    }
}
