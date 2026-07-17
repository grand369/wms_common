using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;
using Wms.Shared.Domain.Enums;

namespace Wms.Outbound.EntityFrameworkCore.Configurations;

/// <summary>
/// OutboundOrder EF Core Configuration (TAB-014) — table and index configuration.
/// Table name: Wms_Outbound_OutboundOrder. Includes unique index on OutboundOrderNo,
/// query indexes on (WarehouseId, OutboundStatus), (OutboundType, OutboundStatus),
/// (IsEmergency), and (CreationTime DESC). Decimal precision: (18,4).
/// </summary>
public class OutboundOrderConfiguration : IEntityTypeConfiguration<OutboundOrder>
{
    public void Configure(EntityTypeBuilder<OutboundOrder> builder)
    {
        builder.ToTable("Wms_Outbound_OutboundOrder");
        builder.HasKey(e => e.Id);

        // UK index on OutboundOrderNo (IDX-016)
        builder.HasIndex(e => e.OutboundOrderNo)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_OB_OutboundOrderNo");

        // IDX-017: WarehouseId + OutboundStatus
        builder.HasIndex(e => new { e.WarehouseId, e.OutboundStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_OB_Order_WarehouseStatus");

        // IDX-018: OutboundType + OutboundStatus
        builder.HasIndex(e => new { e.OutboundType, e.OutboundStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_OB_Order_TypeStatus");

        // IDX-019: IsEmergency (for emergency order filtering)
        builder.HasIndex(e => e.IsEmergency)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_OB_Order_IsEmergency");

        // IDX-020: CreationTime DESC
        builder.HasIndex(e => e.CreationTime)
            .HasName("IDX_OB_Order_CreationTime");

        // Property configurations
        builder.Property(e => e.OutboundOrderNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.OutboundType).IsRequired()
            .HasConversion(s => s.Value, v => OutboundType.FromValue(v));
        builder.Property(e => e.OutboundStatus).IsRequired()
            .HasConversion(s => s.Value, v => OutboundStatus.FromValue(v));
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.MaterialRequisitionId).IsRequired(false);
        builder.Property(e => e.SalesOrderId).IsRequired(false);
        builder.Property(e => e.ReturnMaterialOrderId).IsRequired(false);
        builder.Property(e => e.OverIssueRatio).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.IsEmergency).IsRequired();
        builder.Property(e => e.TotalRequiredQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.TotalAllocatedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.TotalPickedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.TotalShippedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.IsCompleted).IsRequired();
        builder.Property(e => e.CompletionTime).IsRequired(false);
        builder.Property(e => e.ErpCallbackStatus).IsRequired()
            .HasConversion(s => s.Value, v => ErpCallbackStatus.FromValue(v));
        builder.Property(e => e.Remark).HasMaxLength(1000).IsRequired(false);

        // Navigation: Lines — one-to-many with cascade delete
        builder.HasMany(e => e.Lines)
            .WithOne()
            .HasForeignKey(e => e.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
