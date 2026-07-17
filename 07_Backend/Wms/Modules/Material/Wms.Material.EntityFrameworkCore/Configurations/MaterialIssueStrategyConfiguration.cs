using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Material.Domain.Aggregates;

namespace Wms.Material.EntityFrameworkCore.Configurations;

public class MaterialIssueStrategyConfiguration : IEntityTypeConfiguration<MaterialIssueStrategy>
{
    public void Configure(EntityTypeBuilder<MaterialIssueStrategy> builder)
    {
        builder.ToTable("Wms_Material_MaterialIssueStrategy");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_MT_IssueStrategyCode");

        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Strategy).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
