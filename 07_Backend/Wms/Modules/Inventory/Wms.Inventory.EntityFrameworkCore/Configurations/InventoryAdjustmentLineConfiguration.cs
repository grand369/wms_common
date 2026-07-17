using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;
using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Adjustment Line EF Core Configuration (TAB-010a) — adjustment line items table.
/// </summary>
public class InventoryAdjustmentLineConfiguration : IEntityTypeConfiguration<InventoryAdjustmentLine>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustmentLine> builder)
    {
        builder.ToTable("Wms_Inventory_InventoryAdjustmentLine");
        builder.HasKey(e => e.Id);

        // Property configurations
        builder.Property(e => e.AdjustmentId).IsRequired();
        builder.Property(e => e.LineNo).IsRequired();
        builder.Property(e => e.MaterialId).IsRequired();
        builder.Property(e => e.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.MaterialName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.AdjustmentQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.LocationId).IsRequired();
        builder.Property(e => e.LocationCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.BatchNumber).HasMaxLength(50).IsRequired(false);

        builder.Property(e => e.InventoryStatusBefore).IsRequired()
            .HasConversion(s => s.Value, v => InventoryStatus.FromValue(v));
        builder.Property(e => e.InventoryStatusAfter).IsRequired()
            .HasConversion(s => s.Value, v => InventoryStatus.FromValue(v));

        builder.Property(e => e.Reason).HasMaxLength(500).IsRequired(false);
    }
}
