using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SupplierAgg = Wms.Supplier.Domain.Aggregates.Supplier;

namespace Wms.Supplier.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class WmsSupplierDbContext : AbpDbContext<WmsSupplierDbContext>
{
    public DbSet<SupplierAgg> Suppliers { get; set; } = null!;

    public WmsSupplierDbContext(DbContextOptions<WmsSupplierDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSupplier();
    }
}
