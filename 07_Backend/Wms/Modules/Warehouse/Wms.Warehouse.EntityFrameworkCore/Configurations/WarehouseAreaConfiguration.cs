using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Warehouse.Domain.Aggregates;

namespace Wms.Warehouse.EntityFrameworkCore.Configurations;

/// <summary>
/// Warehouse Area EF Core Configuration — configures table name, constraints, indexes.
/// Table: Wms_Warehouse_WarehouseArea
/// (TAB-002, Phase 5 Database Design)
/// </summary>
public class WarehouseAreaConfiguration : IEntityTypeConfiguration<WarehouseArea>
{
    public void Configure(EntityTypeBuilder<WarehouseArea> builder)
    {
        builder.ToTable("Wms_Warehouse_WarehouseArea");
        builder.HasKey(e => e.Id);

        // Composite unique index on (WarehouseId, AreaCode)
        builder.HasIndex(e => new { e.WarehouseId, e.AreaCode })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_WH_AreaCode_Warehouse");

        // Index on WarehouseId
        builder.HasIndex(e => e.WarehouseId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_WH_Area_WarehouseId");

        // Property configurations
        builder.Property(e => e.AreaCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AreaName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.WarehouseId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.WarehouseCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AreaFunction)
            .IsRequired();

        builder.Property(e => e.StorageEnvironment)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.MaxCapacity)
            .HasColumnType("decimal(18,4)")
            .IsRequired(false);

        builder.Property(e => e.CurrentCapacity)
            .HasColumnType("decimal(18,4)")
            .IsRequired(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
