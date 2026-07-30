using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Snapshot EF Core Configuration (TAB-012).
/// </summary>
public class InventorySnapshotConfiguration : IEntityTypeConfiguration<InventorySnapshot>
{
    public void Configure(EntityTypeBuilder<InventorySnapshot> builder)
    {
        builder.ToTable("Wms_Inventory_InventorySnapshot");
        builder.HasKey(e => e.Id);

        // Unique index on SnapshotNo
        builder.HasIndex(e => e.SnapshotNo)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_IV_SnapshotNo");

        // Property configurations
        builder.Property(e => e.SnapshotNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.SnapshotTime).IsRequired();
        builder.Property(e => e.TotalQty).IsRequired().HasPrecision(18, 4);
        builder.Property(e => e.TotalFrozenQty).IsRequired().HasPrecision(18, 4);
        builder.Property(e => e.TotalAvailableQty).IsRequired().HasPrecision(18, 4);
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.Remark).HasMaxLength(1000).IsRequired(false);
    }
}
