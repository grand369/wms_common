using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Warehouse.Domain.Aggregates;

namespace Wms.Warehouse.EntityFrameworkCore.Configurations;

/// <summary>
/// Location EF Core Configuration — configures table name, constraints, indexes.
/// Table: Wms_Warehouse_Location
/// (TAB-003, Phase 5 Database Design)
/// </summary>
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Wms_Warehouse_Location");
        builder.HasKey(e => e.Id);

        // Unique index on LocationCode
        builder.HasIndex(e => e.LocationCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_WH_LocationCode");

        // Index on (WarehouseId, AreaId) for querying locations by warehouse and area
        builder.HasIndex(e => new { e.WarehouseId, e.AreaId })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_WH_Location_WarehouseArea");

        // Index on BarcodeId for PDA scanning lookup
        builder.HasIndex(e => e.BarcodeId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_WH_Location_BarcodeId");

        // Property configurations
        builder.Property(e => e.LocationCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.WarehouseId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.WarehouseCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AreaId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AreaCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.LocationType)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.MaxWeight)
            .HasColumnType("decimal(18,4)")
            .IsRequired(false);

        builder.Property(e => e.MaxCapacity)
            .HasColumnType("decimal(18,4)")
            .IsRequired(false);

        builder.Property(e => e.CurrentWeight)
            .HasColumnType("decimal(18,4)")
            .IsRequired(false);

        builder.Property(e => e.CurrentCapacity)
            .HasColumnType("decimal(18,4)")
            .IsRequired(false);

        builder.Property(e => e.StorageCondition)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.BarcodeId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Row)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(e => e.Column)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(e => e.Layer)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
