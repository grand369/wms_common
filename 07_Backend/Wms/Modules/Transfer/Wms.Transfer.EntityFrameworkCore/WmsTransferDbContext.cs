using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Transfer.Domain.Aggregates;

namespace Wms.Transfer.EntityFrameworkCore;

/// <summary>
/// WmsTransferDbContext — EF Core context for Transfer module.
/// Registers TransferOrder + TransferLine DbSets.
/// </summary>
public class WmsTransferDbContext : AbpDbContext<WmsTransferDbContext>
{
    public DbSet<TransferOrder> TransferOrders { get; set; }
    public DbSet<TransferLine> TransferLines { get; set; }

    public WmsTransferDbContext(DbContextOptions<WmsTransferDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<TransferOrder>(b =>
        {
            b.ToTable("Wms_Transfer_TransferOrders");
            b.HasKey(o => o.Id);
            b.Property(o => o.TransferOrderNo).IsRequired().HasMaxLength(50);
            b.Property(o => o.TransferType).IsRequired();
            b.Property(o => o.TransferStatus).IsRequired();
            b.Property(o => o.SourceWarehouseId).IsRequired();
            b.Property(o => o.SourceWarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(o => o.TargetWarehouseId).IsRequired();
            b.Property(o => o.TargetWarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(o => o.ApprovalStatus).IsRequired();
            b.Property(o => o.IsCrossCompany).IsRequired();
            b.Property(o => o.Remark).HasMaxLength(1000);
            b.HasIndex(o => o.TransferOrderNo).IsUnique().HasFilter(null).HasDatabaseName("UK_TF_TransferOrderNo");
            b.HasIndex(o => o.TransferStatus).HasDatabaseName("IDX_TF_Status");
            b.HasIndex(o => o.SourceWarehouseId).HasDatabaseName("IDX_TF_SourceWarehouse");
            b.HasMany(o => o.Lines).WithOne().HasForeignKey(l => l.TransferOrderId).IsRequired();
        });
        builder.Entity<TransferLine>(b =>
        {
            b.ToTable("Wms_Transfer_TransferLines");
            b.HasKey(l => l.Id);
            b.Property(l => l.LineNo).IsRequired();
            b.Property(l => l.MaterialId).IsRequired();
            b.Property(l => l.MaterialCode).IsRequired().HasMaxLength(50);
            b.Property(l => l.TransferQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(l => l.OutboundConfirmedQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(l => l.InboundConfirmedQuantity).HasColumnType("decimal(18,6)").IsRequired();
        });
    }
}
