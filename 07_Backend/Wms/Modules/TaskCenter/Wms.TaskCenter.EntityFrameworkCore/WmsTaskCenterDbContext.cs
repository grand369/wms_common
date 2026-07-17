using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Wms.TaskCenter.Domain.Aggregates;

namespace Wms.TaskCenter.EntityFrameworkCore;

/// <summary>
/// Updated WmsTaskCenterDbContext — registers DbSet and applies EF configurations.
/// </summary>
public class WmsTaskCenterDbContext : AbpDbContext<WmsTaskCenterDbContext>
{
    public DbSet<WarehouseTask> WarehouseTasks { get; set; }

    public WmsTaskCenterDbContext(DbContextOptions<WmsTaskCenterDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WarehouseTask>(b =>
        {
            b.ToTable("Wms_TaskCenter_WarehouseTask");

            // ── Properties ──
            b.Property(t => t.TaskNo).IsRequired().HasMaxLength(50);
            b.Property(t => t.SourceOrderType).IsRequired().HasMaxLength(50);
            b.Property(t => t.SourceOrderNo).IsRequired().HasMaxLength(50);
            b.Property(t => t.WarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(t => t.AssignedUserName).HasMaxLength(100);
            b.Property(t => t.SuspendedReason).HasMaxLength(500);
            b.Property(t => t.Remark).HasMaxLength(1000);
            b.Property(t => t.TaskProgress).HasColumnType("decimal(5,2)");

            // ── SmartEnum → int conversions ──
            b.Property(t => t.TaskType).HasConversion(e => e.Value, v => Wms.Shared.Domain.Enums.TaskType.FromValue(v));
            b.Property(t => t.TaskPriority).HasConversion(e => e.Value, v => Wms.Shared.Domain.Enums.TaskPriority.FromValue(v));
            b.Property(t => t.TaskStatus).HasConversion(e => e.Value, v => Wms.TaskCenter.Domain.Enums.TaskStatus.FromValue(v));
            b.Property(t => t.AssignmentStrategy).HasConversion(e => e.Value, v => Wms.TaskCenter.Domain.Enums.AssignmentStrategy.FromValue(v));

            // ── Foreign Key (nullable) ──
            b.Property(t => t.AssignedUserId).IsRequired(false);

            // ── Indexes ──
            // IDX-019: UK_TC_TaskNo
            b.HasIndex(t => t.TaskNo).IsUnique().HasFilter("[IsDeleted] = 0");

            // IDX-020: IDX_TC_Task_WarehouseStatus
            b.HasIndex(t => new { t.WarehouseId, t.TaskStatus });

            // IDX-021: IDX_TC_Task_AssignedUser
            b.HasIndex(t => new { t.AssignedUserId, t.TaskStatus });

            // IDX-022: IDX_TC_Task_SourceOrder
            b.HasIndex(t => new { t.SourceOrderType, t.SourceOrderId });

            // IDX-023: IDX_TC_Task_Priority
            b.HasIndex(t => new { t.TaskPriority, t.TaskStatus });

            // IDX-024: IDX_TC_Task_ExpectedTime
            b.HasIndex(t => new { t.ExpectedCompletionTime, t.TaskStatus });
        });
    }
}
