using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;
using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Balance EF Core Configuration (TAB-008) — the core table configuration.
/// Includes composite unique index, query indexes, concurrency token, decimal precision.
/// </summary>
public class InventoryBalanceConfiguration : IEntityTypeConfiguration<InventoryBalance>
{
    public void Configure(EntityTypeBuilder<InventoryBalance> builder)
    {
        builder.ToTable("Wms_Inventory_InventoryBalance");
        builder.HasKey(e => e.Id);

        // ⚠️ Core composite unique index — (MaterialId, WarehouseId, LocationId, BatchNumber, InventoryStatus)
        // Note: BatchNumber is nullable. SQL Server treats NULLs as distinct in unique indexes,
        // so we use HasFilter to handle NULL BatchNumber correctly.
        builder.HasIndex(e => new { e.MaterialId, e.WarehouseId, e.LocationId, e.BatchNumber, e.InventoryStatus })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_IV_Balance_Composite");

        // Query indexes
        builder.HasIndex(e => new { e.MaterialId, e.WarehouseId, e.InventoryStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Balance_MaterialWarehouse");

        builder.HasIndex(e => new { e.WarehouseId, e.InventoryStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Balance_Warehouse");

        builder.HasIndex(e => new { e.MaterialId, e.InventoryStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Balance_MaterialStatus");

        builder.HasIndex(e => e.ExpiryDate)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Balance_Expiry");

        builder.HasIndex(e => e.BatchNumber)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IV_Balance_Batch");

        builder.HasIndex(e => e.LastOperationTime)
            .HasName("IDX_IV_Balance_LastOpTime");

        // Property configurations
        builder.Property(e => e.MaterialId).IsRequired();
        builder.Property(e => e.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.LocationId).IsRequired();
        builder.Property(e => e.LocationCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.BatchNumber).HasMaxLength(50);
        builder.Property(e => e.InventoryStatus).IsRequired()
            .HasConversion(
                s => s.Value,
                v => InventoryStatus.FromValue(v));

        builder.Property(e => e.Quantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.ReservedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.FrozenQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.InTransitQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.AvailableQuantity).HasColumnType("decimal(18,4)").IsRequired();

        builder.Property(e => e.ExpiryDate).IsRequired(false);
        builder.Property(e => e.ProductionDate).IsRequired(false);
        builder.Property(e => e.SupplierId).IsRequired(false);
        builder.Property(e => e.SupplierName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.UnitCost).HasColumnType("decimal(18,6)").IsRequired(false);
        builder.Property(e => e.SafetyStockQuantity).HasColumnType("decimal(18,4)").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.LastOperationTime).IsRequired();

        // ⚠️ Optimistic lock — ConcurrencyVersion as concurrency token
        builder.Property(e => e.ConcurrencyVersion).IsConcurrencyToken().IsRequired();
    }
}
