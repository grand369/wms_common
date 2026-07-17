using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Production.Domain.Aggregates;

namespace Wms.Production.EntityFrameworkCore;

public class WmsProductionDbContext : AbpDbContext<WmsProductionDbContext>
{
    public DbSet<MaterialRequisition> MaterialRequisitions { get; set; }
    public DbSet<MaterialRequisitionLine> MaterialRequisitionLines { get; set; }
    public DbSet<SubcontractOrder> SubcontractOrders { get; set; }

    public WmsProductionDbContext(DbContextOptions<WmsProductionDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<MaterialRequisition>(b =>
        {
            b.ToTable("Wms_Production_MaterialRequisitions");
            b.HasKey(r => r.Id);
            b.Property(r => r.RequisitionNo).IsRequired().HasMaxLength(50);
            b.Property(r => r.ProductionOrderId).IsRequired();
            b.Property(r => r.ProductionOrderNo).IsRequired().HasMaxLength(50);
            b.Property(r => r.RequisitionStatus).IsRequired();
            b.Property(r => r.WarehouseId).IsRequired();
            b.Property(r => r.WarehouseCode).IsRequired().HasMaxLength(50);
            b.HasIndex(r => r.RequisitionNo).IsUnique().HasFilter(null).HasDatabaseName("UK_PD_RequisitionNo");
            b.HasIndex(r => r.ProductionOrderId).HasDatabaseName("IDX_PD_ProductionOrder");
            b.HasMany(r => r.Lines).WithOne().HasForeignKey(l => l.RequisitionId).IsRequired();
        });
        builder.Entity<MaterialRequisitionLine>(b =>
        {
            b.ToTable("Wms_Production_MaterialRequisitionLines");
            b.HasKey(l => l.Id);
            b.Property(l => l.LineNo).IsRequired();
            b.Property(l => l.MaterialId).IsRequired();
            b.Property(l => l.MaterialCode).IsRequired().HasMaxLength(50);
            b.Property(l => l.RequiredQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(l => l.IssuedQuantity).HasColumnType("decimal(18,6)").IsRequired();
        });
        builder.Entity<SubcontractOrder>(b =>
        {
            b.ToTable("Wms_Production_SubcontractOrders");
            b.HasKey(s => s.Id);
            b.Property(s => s.SubcontractOrderNo).IsRequired().HasMaxLength(50);
            b.Property(s => s.VendorId).IsRequired();
            b.Property(s => s.VendorName).IsRequired().HasMaxLength(200);
            b.Property(s => s.SentQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(s => s.ReceivedQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(s => s.LossRate).HasColumnType("decimal(18,6)").IsRequired();
        });
    }
}
