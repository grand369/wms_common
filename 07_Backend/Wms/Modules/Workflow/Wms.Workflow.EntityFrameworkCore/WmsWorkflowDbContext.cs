using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore;
using Wms.Workflow.Domain.Aggregates;

namespace Wms.Workflow.EntityFrameworkCore;

public class WmsWorkflowDbContext : AbpDbContext<WmsWorkflowDbContext>
{
    public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
    public DbSet<ApprovalInstance> ApprovalInstances { get; set; }

    public WmsWorkflowDbContext(DbContextOptions<WmsWorkflowDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ApprovalFlow
        builder.Entity<ApprovalFlow>(b =>
        {
            b.ToTable("WmsApprovalFlows");
            b.HasKey(f => f.Id);

            b.Property(f => f.FlowName).IsRequired().HasMaxLength(100);
            b.Property(f => f.FlowType).IsRequired().HasConversion<int>();
            b.Property(f => f.IsActive).IsRequired();
            b.Property(f => f.Description).HasMaxLength(500);

            b.HasIndex(f => f.FlowName).HasDatabaseName("IDX_WF_FlowName");
            b.HasIndex(f => f.FlowType).HasDatabaseName("IDX_WF_FlowType");
            b.HasIndex(f => f.IsActive).HasDatabaseName("IDX_WF_IsActive");

            b.OwnsMany(f => f.Nodes, (OwnedNavigationBuilder<ApprovalFlow, ApprovalNode> nodeBuilder) =>
            {
                nodeBuilder.ToTable("WmsApprovalNodes");
                nodeBuilder.WithOwner().HasForeignKey(n => n.FlowId);
                nodeBuilder.HasKey(n => n.Id);

                nodeBuilder.Property(n => n.NodeName).IsRequired().HasMaxLength(100);
                nodeBuilder.Property(n => n.NodeType).IsRequired().HasConversion<int>();
                nodeBuilder.Property(n => n.ApproverRole).HasMaxLength(100);
                nodeBuilder.Property(n => n.ApproverUserId);
                nodeBuilder.Property(n => n.ConditionExpression).HasMaxLength(1000);
                nodeBuilder.Property(n => n.Order).IsRequired();
                nodeBuilder.Property(n => n.IsRequired).IsRequired();
            });
        });

        // ApprovalInstance
        builder.Entity<ApprovalInstance>(b =>
        {
            b.ToTable("WmsApprovalInstances");
            b.HasKey(i => i.Id);

            b.Property(i => i.FlowId).IsRequired();
            b.Property(i => i.FlowName).HasMaxLength(100);
            b.Property(i => i.InstanceStatus).IsRequired().HasConversion<int>();
            b.Property(i => i.BusinessOrderId).IsRequired();
            b.Property(i => i.BusinessOrderType).IsRequired().HasMaxLength(50);
            b.Property(i => i.BusinessOrderNo).HasMaxLength(100);
            b.Property(i => i.CurrentNodeId);
            b.Property(i => i.CurrentNodeName).HasMaxLength(100);
            b.Property(i => i.SubmitUserId).IsRequired();
            b.Property(i => i.SubmitUserName).HasMaxLength(100);
            b.Property(i => i.SubmitTime).IsRequired();
            b.Property(i => i.CompletedTime);

            b.HasIndex(i => new { i.BusinessOrderType, i.BusinessOrderId }).HasDatabaseName("IDX_WF_BusinessOrder");
            b.HasIndex(i => i.InstanceStatus).HasDatabaseName("IDX_WF_InstanceStatus");
            b.HasIndex(i => i.SubmitUserId).HasDatabaseName("IDX_WF_SubmitUserId");
            b.HasIndex(i => i.CurrentNodeId).HasDatabaseName("IDX_WF_CurrentNodeId");

            b.OwnsMany(i => i.ActionLogs, (OwnedNavigationBuilder<ApprovalInstance, ApprovalActionLog> logBuilder) =>
            {
                logBuilder.ToTable("WmsApprovalActionLogs");
                logBuilder.WithOwner().HasForeignKey(l => l.InstanceId);
                logBuilder.HasKey(l => l.Id);

                logBuilder.Property(l => l.NodeId).IsRequired();
                logBuilder.Property(l => l.NodeName).HasMaxLength(100);
                logBuilder.Property(l => l.ActionUserId).IsRequired();
                logBuilder.Property(l => l.ActionUserName).HasMaxLength(100);
                logBuilder.Property(l => l.ActionType).IsRequired().HasConversion<int>();
                logBuilder.Property(l => l.Comment).HasMaxLength(1000);
                logBuilder.Property(l => l.ActionTime).IsRequired();
            });
        });
    }
}
