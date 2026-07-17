using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using Wms.Shared.Domain.Enums;

namespace Wms.Inbound.EntityFrameworkCore.Configurations;

/// <summary>
/// InboundOrder EF Core Configuration (TAB-013) — table and index configuration.
/// Table name: Wms_Inbound_InboundOrder. Includes unique index on InboundOrderNo,
/// query indexes on (WarehouseId, InboundStatus), (InboundType, InboundStatus),
/// and (CreationTime DESC). Decimal precision: (18,4).
/// </summary>
public class InboundOrderConfiguration : IEntityTypeConfiguration<InboundOrder>
{
    public void Configure(EntityTypeBuilder<InboundOrder> builder)
    {
        builder.ToTable("Wms_Inbound_InboundOrder");
        builder.HasKey(e => e.Id);

        // UK index on InboundOrderNo (IDX-012)
        builder.HasIndex(e => e.InboundOrderNo)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasName("UK_IN_InboundOrderNo");

        // IDX-013: WarehouseId + InboundStatus
        builder.HasIndex(e => new { e.WarehouseId, e.InboundStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IN_Order_WarehouseStatus");

        // IDX-014: InboundType + InboundStatus
        builder.HasIndex(e => new { e.InboundType, e.InboundStatus })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_IN_Order_TypeStatus");

        // IDX-015: CreationTime DESC
        builder.HasIndex(e => e.CreationTime)
            .HasName("IDX_IN_Order_CreationTime");

        // Property configurations
        builder.Property(e => e.InboundOrderNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.InboundType).IsRequired()
            .HasConversion(s => s.Value, v => InboundType.FromValue(v));
        builder.Property(e => e.InboundStatus).IsRequired()
            .HasConversion(s => s.Value, v => InboundStatus.FromValue(v));
        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.PurchaseOrderId).IsRequired(false);
        builder.Property(e => e.PurchaseOrderNo).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.ProductionOrderId).IsRequired(false);
        builder.Property(e => e.ReturnOrderId).IsRequired(false);
        builder.Property(e => e.SupplierId).IsRequired(false);
        builder.Property(e => e.SupplierName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.OverReceiptRatio).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.QualityInspectionRequired).IsRequired();
        builder.Property(e => e.TotalPlanQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.TotalReceivedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.IsCompleted).IsRequired();
        builder.Property(e => e.CompletionTime).IsRequired(false);
        builder.Property(e => e.ErpCallbackStatus).IsRequired()
            .HasConversion(s => s.Value, v => ErpCallbackStatus.FromValue(v));
        builder.Property(e => e.Remark).HasMaxLength(1000).IsRequired(false);

        // Navigation: Lines — one-to-many with cascade delete
        builder.HasMany(e => e.Lines)
            .WithOne()
            .HasForeignKey(e => e.InboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
