using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Adjustment EF Core Configuration (TAB-010) — adjustment header table.
/// </summary>
public class InventoryAdjustmentConfiguration : IEntityTypeConfiguration<InventoryAdjustment>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustment> builder)
    {
        builder.ToTable("Wms_Inventory_InventoryAdjustment");
        builder.HasKey(e => e.Id);

        // Unique index on AdjustmentNo
        builder.HasIndex(e => e.AdjustmentNo)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_IV_AdjustmentNo");

        // Query index on WarehouseId
        builder.HasIndex(e => e.WarehouseId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Adjustment_Warehouse");

        // Property configurations
        builder.Property(e => e.AdjustmentNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.AdjustmentType).IsRequired()
            .HasConversion(t => t.Value, v => AdjustmentType.FromValue(v));
        builder.Property(e => e.AdjustmentReason).IsRequired().HasMaxLength(500);
        builder.Property(e => e.ApprovalStatus).IsRequired()
            .HasConversion(t => t.Value, v => AdjustmentApprovalStatus.FromValue(v));
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.IsCompleted).IsRequired();
        builder.Property(e => e.CompletionTime).IsRequired(false);
        builder.Property(e => e.Remark).HasMaxLength(1000).IsRequired(false);

        // Navigation — Lines collection
        builder.HasMany(e => e.Lines)
            .WithOne()
            .HasForeignKey(l => l.AdjustmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
