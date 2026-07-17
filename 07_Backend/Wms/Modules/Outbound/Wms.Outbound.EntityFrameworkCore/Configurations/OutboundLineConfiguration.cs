using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;

namespace Wms.Outbound.EntityFrameworkCore.Configurations;

/// <summary>
/// OutboundLine EF Core Configuration (TAB-014a) — child entity of OutboundOrder.
/// Table name: Wms_Outbound_OutboundLine. Decimal precision: (18,4).
/// </summary>
public class OutboundLineConfiguration : IEntityTypeConfiguration<OutboundLine>
{
    public void Configure(EntityTypeBuilder<OutboundLine> builder)
    {
        builder.ToTable("Wms_Outbound_OutboundLine");
        builder.HasKey(e => e.Id);

        // Index on OutboundOrderId for parent lookup
        builder.HasIndex(e => e.OutboundOrderId)
            .HasName("IDX_OB_Line_OutboundOrderId");

        // Property configurations
        builder.Property(e => e.OutboundOrderId).IsRequired();
        builder.Property(e => e.LineNo).IsRequired();
        builder.Property(e => e.MaterialId).IsRequired();
        builder.Property(e => e.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.MaterialName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.RequiredQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.AllocatedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.PickedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.ShippedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.PickingLocationId).IsRequired(false);
        builder.Property(e => e.PickingLocationCode).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.IssueStrategy).IsRequired()
            .HasConversion(s => s.Value, v => IssueStrategyType.FromValue(v));
        builder.Property(e => e.BatchNumber).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.Remark).HasMaxLength(500).IsRequired(false);
    }
}
