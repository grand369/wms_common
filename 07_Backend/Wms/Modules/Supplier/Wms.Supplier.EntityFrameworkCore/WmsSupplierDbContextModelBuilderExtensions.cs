using Microsoft.EntityFrameworkCore;
using Wms.Supplier.EntityFrameworkCore.Configurations;

namespace Wms.Supplier.EntityFrameworkCore;

/// <summary>
/// Supplier Module ModelBuilder Extensions — registers entity configurations.
/// </summary>
public static class WmsSupplierDbContextModelBuilderExtensions
{
    public static void ConfigureSupplier(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new SupplierConfiguration());
    }
}
