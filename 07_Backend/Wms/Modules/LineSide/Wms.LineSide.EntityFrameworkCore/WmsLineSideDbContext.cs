using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.LineSide.Domain.Aggregates;

namespace Wms.LineSide.EntityFrameworkCore;

public class WmsLineSideDbContext : AbpDbContext<WmsLineSideDbContext>
{
    public DbSet<LineSideWarehouse> LineSideWarehouses { get; set; }
    public DbSet<LineSideKanbanItem> LineSideKanbanItems { get; set; }

    public WmsLineSideDbContext(DbContextOptions<WmsLineSideDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<LineSideWarehouse>(b =>
        {
            b.ToTable("Wms_LineSide_LineSideWarehouses");
            b.HasKey(w => w.Id);
            b.Property(w => w.LineSideWarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(w => w.LineSideWarehouseName).IsRequired().HasMaxLength(200);
            b.Property(w => w.WarehouseId).IsRequired();
            b.Property(w => w.WarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(w => w.ProductionLineId).IsRequired();
            b.Property(w => w.ProductionLineName).IsRequired().HasMaxLength(100);
            b.Property(w => w.WorkStationId);
            b.Property(w => w.ConsumptionMode).IsRequired();
            b.HasIndex(w => w.LineSideWarehouseCode).IsUnique().HasFilter(null).HasDatabaseName("UK_LS_Code");
            b.HasIndex(w => w.ProductionLineId).HasDatabaseName("IDX_LS_ProductionLine");
            b.HasMany(w => w.KanbanItems).WithOne().HasForeignKey(k => k.LineSideWarehouseId).IsRequired();
        });
        builder.Entity<LineSideKanbanItem>(b =>
        {
            b.ToTable("Wms_LineSide_LineSideKanbanItems");
            b.HasKey(k => k.Id);
            b.Property(k => k.MaterialId).IsRequired();
            b.Property(k => k.MaterialCode).IsRequired().HasMaxLength(50);
            b.Property(k => k.MinQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(k => k.MaxQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(k => k.CurrentQuantity).HasColumnType("decimal(18,6)").IsRequired();
        });
    }
}
