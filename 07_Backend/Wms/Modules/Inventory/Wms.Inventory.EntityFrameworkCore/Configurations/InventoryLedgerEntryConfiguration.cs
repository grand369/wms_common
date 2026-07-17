using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.EntityFrameworkCore.Configurations;

/// <summary>
/// Inventory Ledger Entry EF Core Configuration (TAB-009) — immutable record table.
/// ⚠️ CRITICAL: No LastModificationTime, IsDeleted columns. Only CreationTime + CreatorId.
/// </summary>
public class InventoryLedgerEntryConfiguration : IEntityTypeConfiguration<InventoryLedgerEntry>
{
    public void Configure(EntityTypeBuilder<InventoryLedgerEntry> builder)
    {
        builder.ToTable("Wms_Inventory_InventoryLedgerEntry");
        builder.HasKey(e => e.Id);

        // ⚠️ This entity does NOT have LastModificationTime or IsDeleted columns
        // Only CreationTime and CreatorId (inherited from IHasCreationTime)
        // The Repository Update/Delete overrides throw NotSupportedException

        // Indexes
        builder.HasIndex(e => e.InventoryBalanceId)
            .HasName("IDX_IV_Ledger_BalanceId");

        builder.HasIndex(e => new { e.SourceOrderType, e.SourceOrderId })
            .HasName("IDX_IV_Ledger_SourceOrder");

        builder.HasIndex(e => e.OperationTime)
            .HasName("IDX_IV_Ledger_TimeRange");

        builder.HasIndex(e => new { e.InventoryBalanceId, e.OperationTime })
            .HasName("IDX_IV_Ledger_BalanceTime");

        // Property configurations
        builder.Property(e => e.InventoryBalanceId).IsRequired();
        builder.Property(e => e.OperationType).IsRequired()
            .HasConversion(
                t => t.Value,
                v => InventoryOperationType.FromValue(v));

        builder.Property(e => e.OperationQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.BeforeQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.AfterQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.BeforeAvailable).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.AfterAvailable).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.OperationTime).IsRequired();
        builder.Property(e => e.OperatorId).IsRequired();
        builder.Property(e => e.OperatorName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.SourceOrderType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.SourceOrderId).IsRequired();
        builder.Property(e => e.SourceOrderNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Remark).HasMaxLength(500).IsRequired(false);
    }
}
