using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.CycleCount.Domain.Aggregates;

namespace Wms.CycleCount.EntityFrameworkCore;

public class WmsCycleCountDbContext : AbpDbContext<WmsCycleCountDbContext>
{
    public DbSet<CycleCountPlan> CycleCountPlans { get; set; }
    public DbSet<CycleCountItem> CycleCountItems { get; set; }
    public DbSet<CycleCountResult> CycleCountResults { get; set; }

    public WmsCycleCountDbContext(DbContextOptions<WmsCycleCountDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CycleCountPlan>(b =>
        {
            b.ToTable("Wms_CycleCount_CycleCountPlans");
            b.HasKey(p => p.Id);
            b.Property(p => p.PlanNo).IsRequired().HasMaxLength(50);
            b.Property(p => p.CountMethod).IsRequired();
            b.Property(p => p.CountStatus).IsRequired();
            b.Property(p => p.WarehouseId).IsRequired();
            b.Property(p => p.WarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(p => p.PlannedDate).IsRequired();
            b.Property(p => p.FreezeInventory).IsRequired();
            b.Property(p => p.DifferenceThreshold).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(p => p.BlindCountEnabled).IsRequired();
            b.Property(p => p.Remark).HasMaxLength(1000);
            b.HasIndex(p => p.PlanNo).IsUnique().HasFilter(null).HasDatabaseName("UK_CC_PlanNo");
            b.HasIndex(p => p.CountStatus).HasDatabaseName("IDX_CC_Status");
            b.HasIndex(p => p.WarehouseId).HasDatabaseName("IDX_CC_Warehouse");
            b.HasMany(p => p.Items).WithOne().HasForeignKey(i => i.PlanId).IsRequired();
        });

        builder.Entity<CycleCountItem>(b =>
        {
            b.ToTable("Wms_CycleCount_CycleCountItems");
            b.HasKey(i => i.Id);
            b.Property(i => i.LocationId).IsRequired();
            b.Property(i => i.LocationCode).IsRequired().HasMaxLength(50);
            b.Property(i => i.MaterialId).IsRequired();
            b.Property(i => i.MaterialCode).IsRequired().HasMaxLength(50);
            b.Property(i => i.SystemQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(i => i.DifferenceQuantity).HasColumnType("decimal(18,6)").IsRequired();
        });

        builder.Entity<CycleCountResult>(b =>
        {
            b.ToTable("Wms_CycleCount_CycleCountResults");
            b.HasKey(r => r.Id);
            b.Property(r => r.PlanId).IsRequired();
            b.Property(r => r.LocationId).IsRequired();
            b.Property(r => r.MaterialId).IsRequired();
            b.Property(r => r.SystemQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(r => r.ActualQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(r => r.DifferenceQuantity).HasColumnType("decimal(18,6)").IsRequired();
            b.Property(r => r.DifferenceAmount).HasColumnType("decimal(18,6)").IsRequired();
            b.HasIndex(r => r.PlanId).HasDatabaseName("IDX_CC_ResultPlanId");
        });
    }
}
