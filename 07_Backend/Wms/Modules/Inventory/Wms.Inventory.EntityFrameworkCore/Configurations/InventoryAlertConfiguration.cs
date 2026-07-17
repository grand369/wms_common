using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Alert EF Core Configuration (TAB-012).
/// </summary>
public class InventoryAlertConfiguration : IEntityTypeConfiguration<InventoryAlert>
{
    public void Configure(EntityTypeBuilder<InventoryAlert> builder)
    {
        builder.ToTable("Wms_Inventory_InventoryAlert");
        builder.HasKey(e => e.Id);

        // Query index on (MaterialId, WarehouseId, AlertType, IsResolved)
        builder.HasIndex(e => new { e.MaterialId, e.WarehouseId, e.AlertType, e.IsResolved })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Alert_MaterialWarehouse");

        // Property configurations
        builder.Property(e => e.AlertType).IsRequired()
            .HasConversion(t => t.Value, v => AlertType.FromValue(v));
        builder.Property(e => e.MaterialId).IsRequired();
        builder.Property(e => e.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CurrentQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.ThresholdQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.IsResolved).IsRequired();
        builder.Property(e => e.AlertTime).IsRequired();
        builder.Property(e => e.ResolveTime).IsRequired(false);
    }
}
