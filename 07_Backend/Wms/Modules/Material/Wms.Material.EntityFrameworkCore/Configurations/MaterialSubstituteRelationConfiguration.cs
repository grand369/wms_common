using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Material.Domain.Aggregates;

namespace Wms.Material.EntityFrameworkCore.Configurations;

/// <summary>
/// Material Substitute Relation EF Core Configuration.
/// Table: Wms_Material_MaterialSubstituteRelation (独立表，而非 Owned Entity)
/// (TAB-006, Phase 5 Database Design)
/// </summary>
public class MaterialSubstituteRelationConfiguration : IEntityTypeConfiguration<MaterialSubstituteRelation>
{
    public void Configure(EntityTypeBuilder<MaterialSubstituteRelation> builder)
    {
        builder.ToTable("Wms_Material_MaterialSubstituteRelation");
        builder.HasKey(e => e.Id);

        // Composite unique index on (OriginalMaterialId, SubstituteMaterialId)
        builder.HasIndex(e => new { e.OriginalMaterialId, e.SubstituteMaterialId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_MT_Substitute_Composite");

        builder.Property(e => e.OriginalMaterialId).IsRequired();
        builder.Property(e => e.SubstituteMaterialId).IsRequired();
        builder.Property(e => e.SubstituteMaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.SubstitutePriority).IsRequired().HasDefaultValue(1);
        builder.Property(e => e.SubstituteRatio).HasColumnType("decimal(18,6)").IsRequired().HasDefaultValue(1.0);
    }
}
