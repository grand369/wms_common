using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using System.Text.Json;

namespace Wms.Inbound.EntityFrameworkCore.Configurations;

/// <summary>
/// InboundLine EF Core Configuration (TAB-013a) — child entity of InboundOrder.
/// Table name: Wms_Inbound_InboundLine. SerialNumberList stored as nvarchar(max) JSON.
/// Decimal precision: (18,4).
/// </summary>
public class InboundLineConfiguration : IEntityTypeConfiguration<InboundLine>
{
    public void Configure(EntityTypeBuilder<InboundLine> builder)
    {
        builder.ToTable("Wms_Inbound_InboundLine");
        builder.HasKey(e => e.Id);

        // Index on InboundOrderId for parent lookup
        builder.HasIndex(e => e.InboundOrderId)
            .HasName("IDX_IN_Line_InboundOrderId");

        // Property configurations
        builder.Property(e => e.InboundOrderId).IsRequired();
        builder.Property(e => e.LineNo).IsRequired();
        builder.Property(e => e.MaterialId).IsRequired();
        builder.Property(e => e.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.MaterialName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PlanQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.BatchNumber).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.QualityStatus).IsRequired()
            .HasConversion(s => s.Value, v => QualityStatus.FromValue(v));
        builder.Property(e => e.PutawayLocationId).IsRequired(false);
        builder.Property(e => e.PutawayLocationCode).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.ExpiryDate).IsRequired(false);
        builder.Property(e => e.ProductionDate).IsRequired(false);
        builder.Property(e => e.Remark).HasMaxLength(500).IsRequired(false);

        // ⚠️ SerialNumberList — stored as nvarchar(max) JSON
        builder.Property(e => e.SerialNumberList)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null));
    }
}
