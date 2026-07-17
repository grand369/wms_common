using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Freeze Order EF Core Configuration (TAB-011).
/// </summary>
public class InventoryFreezeOrderConfiguration : IEntityTypeConfiguration<InventoryFreezeOrder>
{
    public void Configure(EntityTypeBuilder<InventoryFreezeOrder> builder)
    {
        builder.ToTable("Wms_Inventory_InventoryFreezeOrder");
        builder.HasKey(e => e.Id);

        // Unique index on FreezeOrderNo
        builder.HasIndex(e => e.FreezeOrderNo)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_IV_FreezeOrderNo");

        // Property configurations
        builder.Property(e => e.FreezeOrderNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.FreezeScope).IsRequired()
            .HasConversion(t => t.Value, v => FreezeScope.FromValue(v));
        builder.Property(e => e.FreezeReason).IsRequired().HasMaxLength(500);
        builder.Property(e => e.FreezeStatus).IsRequired()
            .HasConversion(t => t.Value, v => FreezeStatus.FromValue(v));
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.IsApproved).IsRequired();
        builder.Property(e => e.FreezeStartTime).IsRequired();
        builder.Property(e => e.FreezeEndTime).IsRequired(false);
        builder.Property(e => e.Remark).HasMaxLength(1000).IsRequired(false);
    }
}
