using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Material.Domain.Entities;

namespace Wms.Material.EntityFrameworkCore.Configurations;

/// <summary>
/// Unit of Measure EF Core Configuration.
/// Table: Wms_Material_UnitOfMeasure
/// (TAB-007, Phase 5 Database Design)
/// </summary>
public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("Wms_Material_UnitOfMeasure");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UnitCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_MT_UnitCode");

        builder.Property(e => e.UnitCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.UnitName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.UnitSymbol).IsRequired().HasMaxLength(20);
        builder.Property(e => e.UnitType).IsRequired();
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
    }
}
