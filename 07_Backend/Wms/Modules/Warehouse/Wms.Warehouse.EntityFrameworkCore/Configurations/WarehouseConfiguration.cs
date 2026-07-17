using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Warehouse.Domain.Aggregates;
using WarehouseEntity = Wms.Warehouse.Domain.Aggregates.Warehouse;

namespace Wms.Warehouse.EntityFrameworkCore.Configurations;

/// <summary>
/// Warehouse EF Core Configuration — configures table name, constraints, indexes.
/// Table: Wms_Warehouse_Warehouse
/// (TAB-001, Phase 5 Database Design)
/// </summary>
public class WarehouseConfiguration : IEntityTypeConfiguration<WarehouseEntity>
{
    public void Configure(EntityTypeBuilder<WarehouseEntity> builder)
    {
        builder.ToTable("Wms_Warehouse_Warehouse");
        builder.HasKey(e => e.Id);

        // Unique index on WarehouseCode
        builder.HasIndex(e => e.WarehouseCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_WH_WarehouseCode");

        // Index on OrganizationUnitId
        builder.HasIndex(e => e.OrganizationUnitId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_WH_OrganizationUnitId");

        // Index on PlantId
        builder.HasIndex(e => e.PlantId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_WH_PlantId");

        // Property configurations
        builder.Property(e => e.WarehouseCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.WarehouseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.WarehouseType)
            .IsRequired();

        builder.Property(e => e.OrganizationUnitId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.OrganizationUnitName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.PlantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.PlantName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ResponsibleUserId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(e => e.ResponsibleUserName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.StorageConditionType)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.LocationLevelCount)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.Remark)
            .HasMaxLength(1000)
            .IsRequired(false);
    }
}
