using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Warehouse.Domain.Aggregates;
using WarehouseAgg = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.EntityFrameworkCore.Configurations;

namespace Wms.Warehouse.EntityFrameworkCore;

/// <summary>
/// Warehouse Module DbContext — v1.0 contributes tables to the shared WmsDbContext.
/// v2.0 can become an independent DbContext when modules are split into microservices.
/// Contains DbSet for Warehouse, WarehouseArea, and Location entities.
/// (Phase 5 Database Design)
/// </summary>
public class WmsWarehouseDbContext : AbpDbContext<WmsWarehouseDbContext>
{
    // BC-01 Warehouse DbSet declarations
    public DbSet<WarehouseAgg> Warehouses { get; set; }
    public DbSet<WarehouseArea> WarehouseAreas { get; set; }
    public DbSet<Location> Locations { get; set; }

    public WmsWarehouseDbContext(DbContextOptions<WmsWarehouseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all Warehouse EF Core configurations
        builder.ApplyConfiguration(new WarehouseConfiguration());
        builder.ApplyConfiguration(new WarehouseAreaConfiguration());
        builder.ApplyConfiguration(new LocationConfiguration());
    }
}
