using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;
using Wms.Material.Domain.ValueObjects;

namespace Wms.Material.EntityFrameworkCore.Configurations;

/// <summary>
/// Material EF Core Configuration — configures table name, constraints, indexes, JSON value object mapping.
/// Table: Wms_Material_Material
/// (TAB-004, Phase 5 Database Design)
/// </summary>
public class MaterialConfiguration : IEntityTypeConfiguration<MaterialAgg>
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public void Configure(EntityTypeBuilder<MaterialAgg> builder)
    {
        builder.ToTable("Wms_Material_Material");
        builder.HasKey(e => e.Id);

        // Unique index on MaterialCode
        builder.HasIndex(e => e.MaterialCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_MT_MaterialCode");

        // Index on ClassificationId
        builder.HasIndex(e => e.ClassificationId)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_MT_Material_Classification");

        // Index on MaterialType
        builder.HasIndex(e => e.MaterialType)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_MT_Material_Type");

        // Index on MaterialName for search
        builder.HasIndex(e => e.MaterialName)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_MT_Material_Name");

        // Property configurations
        builder.Property(e => e.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.MaterialName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.MaterialNameEn).HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.Specification).HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.PrimaryUnitId).IsRequired();
        builder.Property(e => e.PrimaryUnitName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.ConversionRate).HasColumnType("decimal(18,6)").IsRequired(false);
        builder.Property(e => e.MaterialType).IsRequired();
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.ErpSyncStatus).IsRequired().HasDefaultValue(0);

        // JSON column value object mapping for StorageAttribute
        builder.Property(e => e.StorageAttribute)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                vo => JsonSerializer.Serialize(vo, JsonOptions),
                json => JsonSerializer.Deserialize<StorageAttribute>(json, JsonOptions) ?? new StorageAttribute());

        // JSON column value object mapping for QualityAttribute
        builder.Property(e => e.QualityAttribute)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                vo => JsonSerializer.Serialize(vo, JsonOptions),
                json => JsonSerializer.Deserialize<QualityAttribute>(json, JsonOptions) ?? new QualityAttribute());

        // JSON column value object mapping for InventoryAttribute
        builder.Property(e => e.InventoryAttribute)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                vo => JsonSerializer.Serialize(vo, JsonOptions),
                json => JsonSerializer.Deserialize<InventoryAttribute>(json, JsonOptions) ?? new InventoryAttribute());

        // JSON column value object mapping for IssueStrategy
        builder.Property(e => e.IssueStrategy)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                vo => JsonSerializer.Serialize(vo, JsonOptions),
                json => JsonSerializer.Deserialize<IssueStrategy>(json, JsonOptions) ?? new IssueStrategy());

        // JSON column value object mapping for DangerAttribute (nullable)
        builder.Property(e => e.DangerAttribute)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                vo => vo != null ? JsonSerializer.Serialize(vo, JsonOptions) : null,
                json => json != null ? JsonSerializer.Deserialize<DangerAttribute>(json, JsonOptions) : null);

        // Child entity navigation: MaterialSubstituteRelation
        builder.HasMany(e => e.SubstituteRelations)
            .WithOne()
            .HasForeignKey(sr => sr.OriginalMaterialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
