using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Wms.Warehouse.EntityFrameworkCore.Configurations;
using Wms.Material.EntityFrameworkCore.Configurations;
using Wms.Inventory.EntityFrameworkCore.Configurations;
using Wms.Inbound.EntityFrameworkCore.Configurations;
using Wms.Outbound.EntityFrameworkCore.Configurations;
using Wms.BarcodeLabel.EntityFrameworkCore.Configurations;
using Wms.RuleEngine.EntityFrameworkCore.Configurations;
using Wms.Notification.EntityFrameworkCore.Configurations;
using Wms.LineSide.Domain.Aggregates;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.Transfer.Domain.Aggregates;
using Wms.Production.Domain.Aggregates;
using Wms.CycleCount.Domain.Aggregates;
using Wms.Workflow.Domain.Aggregates;
using Wms.TaskCenter.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;

namespace Wms.EntityFrameworkCore;

public class WmsDbContext : AbpDbContext<WmsDbContext>
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options)
        : base(options)
    {
    }

    public DbSet<LineSideWarehouse> LineSideWarehouses { get; set; }
    public DbSet<LineSideKanbanItem> LineSideKanbanItems { get; set; }
    public DbSet<BusinessRule> BusinessRules { get; set; }
    public DbSet<IndustryPackage> IndustryPackages { get; set; }
    public DbSet<TransferOrder> TransferOrders { get; set; }
    public DbSet<TransferLine> TransferLines { get; set; }
    public DbSet<MaterialRequisition> MaterialRequisitions { get; set; }
    public DbSet<MaterialRequisitionLine> MaterialRequisitionLines { get; set; }
    public DbSet<SubcontractOrder> SubcontractOrders { get; set; }
    public DbSet<CycleCountPlan> CycleCountPlans { get; set; }
    public DbSet<CycleCountItem> CycleCountItems { get; set; }
    public DbSet<CycleCountResult> CycleCountResults { get; set; }
    public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
    public DbSet<ApprovalInstance> ApprovalInstances { get; set; }
    public DbSet<WarehouseTask> WarehouseTasks { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<Wms.Notification.Domain.Aggregates.NotificationTemplate> NotificationTemplates { get; set; }
    public DbSet<Wms.Notification.Domain.Aggregates.NotificationRule> NotificationRules { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureIdentity();
        builder.ConfigurePermissionManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureSettingManagement();
        builder.ConfigureFeatureManagement();
        builder.ConfigureAuditLogging();
        builder.ApplyConfiguration(new WarehouseConfiguration());
        builder.ApplyConfiguration(new WarehouseAreaConfiguration());
        builder.ApplyConfiguration(new LocationConfiguration());
        builder.ApplyConfiguration(new MaterialConfiguration());
        builder.ApplyConfiguration(new MaterialClassificationConfiguration());
        builder.ApplyConfiguration(new MaterialSubstituteRelationConfiguration());
        builder.ApplyConfiguration(new UnitOfMeasureConfiguration());
        builder.ApplyConfiguration(new MaterialIssueStrategyConfiguration());
        builder.ApplyConfiguration(new InventoryBalanceConfiguration());
        builder.ApplyConfiguration(new InventoryLedgerEntryConfiguration());
        builder.ApplyConfiguration(new InventoryAdjustmentConfiguration());
        builder.ApplyConfiguration(new InventoryAdjustmentLineConfiguration());
        builder.ApplyConfiguration(new InventoryFreezeOrderConfiguration());
        builder.ApplyConfiguration(new InventoryAlertConfiguration());
        builder.ApplyConfiguration(new InboundOrderConfiguration());
        builder.ApplyConfiguration(new InboundLineConfiguration());
        builder.ApplyConfiguration(new OutboundOrderConfiguration());
        builder.ApplyConfiguration(new OutboundLineConfiguration());
        builder.ApplyConfiguration(new BarcodeRuleConfiguration());
        builder.ApplyConfiguration(new LabelTemplateConfiguration());
        builder.ApplyConfiguration(new PrintTaskConfiguration());
        builder.ApplyConfiguration(new BusinessRuleConfiguration());
        builder.ApplyConfiguration(new IndustryPackageConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new NotificationTemplateConfiguration());
        builder.ApplyConfiguration(new NotificationRuleConfiguration());

        ConfigureLineSide(builder);
        ConfigureTransfer(builder);
        ConfigureProduction(builder);
        ConfigureCycleCount(builder);
        ConfigureWorkflow(builder);
        ConfigureTaskCenter(builder);

        // Step 1: Explicitly configure ExtraProperties value conversion for all WMS entities
        // that inherit FullAuditedAggregateRoot (which implements IHasExtraProperties).
        // This must be done BEFORE ignoring ExtraPropertyDictionary, so that the value
        // conversion overrides the navigation property discovery.
        ConfigureExtraPropertiesValueConversion(builder);

        // Step 2: Ignore ExtraPropertyDictionary as a standalone entity type to prevent
        // EF Core from trying to create a table for it. Since value conversion is already
        // configured for ExtraProperties on business entities, this Ignore only affects
        // the standalone entity discovery and won't remove the value conversion properties.
        builder.Ignore<ExtraPropertyDictionary>();
    }

    private void ConfigureLineSide(ModelBuilder builder)
    {
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
            b.Property(w => w.ConsumptionMode).HasConversion(e => e.Value, v => Wms.LineSide.Domain.Enums.ConsumptionMode.FromValue(v));
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

    private void ConfigureTransfer(ModelBuilder builder)
    {
        builder.Entity<TransferOrder>(b =>
        {
            b.ToTable("Wms_Transfer_TransferOrders");
            b.HasKey(o => o.Id);
            b.Property(o => o.TransferOrderNo).IsRequired().HasMaxLength(50);
            b.Property(o => o.TransferType).HasConversion(e => e.Value, v => Wms.Shared.Domain.Enums.TransferType.FromValue(v));
            b.Property(o => o.TransferStatus).HasConversion(e => e.Value, v => Wms.Transfer.Domain.Enums.TransferStatus.FromValue(v));
            b.Property(o => o.SourceWarehouseId).IsRequired();
            b.Property(o => o.SourceWarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(o => o.TargetWarehouseId).IsRequired();
            b.Property(o => o.TargetWarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(o => o.ApprovalStatus).HasConversion(e => e.Value, v => Wms.Transfer.Domain.Enums.ApprovalStatus.FromValue(v));
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

    private void ConfigureProduction(ModelBuilder builder)
    {
        builder.Entity<MaterialRequisition>(b =>
        {
            b.ToTable("Wms_Production_MaterialRequisitions");
            b.HasKey(r => r.Id);
            b.Property(r => r.RequisitionNo).IsRequired().HasMaxLength(50);
            b.Property(r => r.ProductionOrderId).IsRequired();
            b.Property(r => r.ProductionOrderNo).IsRequired().HasMaxLength(50);
            b.Property(r => r.RequisitionStatus).HasConversion(e => e.Value, v => Wms.Production.Domain.Enums.RequisitionStatus.FromValue(v));
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

    private void ConfigureCycleCount(ModelBuilder builder)
    {
        builder.Entity<CycleCountPlan>(b =>
        {
            b.ToTable("Wms_CycleCount_CycleCountPlans");
            b.HasKey(p => p.Id);
            b.Property(p => p.PlanNo).IsRequired().HasMaxLength(50);
            b.Property(p => p.CountMethod).HasConversion(e => e.Value, v => Wms.CycleCount.Domain.Enums.CountMethod.FromValue(v));
            b.Property(p => p.CountStatus).HasConversion(e => e.Value, v => Wms.CycleCount.Domain.Enums.CountStatus.FromValue(v));
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

    private void ConfigureWorkflow(ModelBuilder builder)
    {
        builder.Entity<ApprovalFlow>(b =>
        {
            b.ToTable("WmsApprovalFlows");
            b.HasKey(f => f.Id);
            b.Property(f => f.FlowName).IsRequired().HasMaxLength(100);
            b.Property(f => f.FlowType).HasConversion(e => e.Value, v => Wms.Workflow.Domain.Enums.ApprovalFlowType.FromValue(v));
            b.Property(f => f.IsActive).IsRequired();
            b.Property(f => f.Description).HasMaxLength(500);
            b.HasIndex(f => f.FlowName).HasDatabaseName("IDX_WF_FlowName");
            b.HasIndex(f => f.FlowType).HasDatabaseName("IDX_WF_FlowType");
            b.HasIndex(f => f.IsActive).HasDatabaseName("IDX_WF_IsActive");
            b.HasMany(f => f.Nodes).WithOne().HasForeignKey(n => n.FlowId).IsRequired();
        });
        builder.Entity<Wms.Workflow.Domain.Aggregates.ApprovalNode>(b =>
        {
            b.ToTable("WmsApprovalNodes");
            b.HasKey(n => n.Id);
            b.Property(n => n.FlowId).IsRequired();
            b.Property(n => n.NodeName).IsRequired().HasMaxLength(100);
            b.Property(n => n.NodeType).HasConversion(e => e.Value, v => Wms.Workflow.Domain.Enums.ApprovalNodeType.FromValue(v));
            b.Property(n => n.ApproverRole).HasMaxLength(100);
            b.Property(n => n.ApproverUserId);
            b.Property(n => n.ConditionExpression).HasMaxLength(1000);
            b.Property(n => n.Order).IsRequired();
            b.Property(n => n.IsRequired).IsRequired();
        });
        builder.Entity<ApprovalInstance>(b =>
        {
            b.ToTable("WmsApprovalInstances");
            b.HasKey(i => i.Id);
            b.Property(i => i.FlowId).IsRequired();
            b.Property(i => i.FlowName).HasMaxLength(100);
            b.Property(i => i.InstanceStatus).HasConversion(e => e.Value, v => Wms.Workflow.Domain.Enums.ApprovalInstanceStatus.FromValue(v));
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
            b.HasMany(i => i.ActionLogs).WithOne().HasForeignKey(l => l.InstanceId).IsRequired();
        });
        builder.Entity<Wms.Workflow.Domain.Aggregates.ApprovalActionLog>(b =>
        {
            b.ToTable("WmsApprovalActionLogs");
            b.HasKey(l => l.Id);
            b.Property(l => l.InstanceId).IsRequired();
            b.Property(l => l.NodeId).IsRequired();
            b.Property(l => l.NodeName).HasMaxLength(100);
            b.Property(l => l.ActionUserId).IsRequired();
            b.Property(l => l.ActionUserName).HasMaxLength(100);
            b.Property(l => l.ActionType).HasConversion(e => e.Value, v => Wms.Workflow.Domain.Enums.ApprovalActionType.FromValue(v));
            b.Property(l => l.Comment).HasMaxLength(1000);
            b.Property(l => l.ActionTime).IsRequired();
        });
    }

    private void ConfigureTaskCenter(ModelBuilder builder)
    {
        builder.Entity<WarehouseTask>(b =>
        {
            b.ToTable("Wms_TaskCenter_WarehouseTask");
            b.Property(t => t.TaskNo).IsRequired().HasMaxLength(50);
            b.Property(t => t.SourceOrderType).IsRequired().HasMaxLength(50);
            b.Property(t => t.SourceOrderNo).IsRequired().HasMaxLength(50);
            b.Property(t => t.WarehouseCode).IsRequired().HasMaxLength(50);
            b.Property(t => t.AssignedUserName).HasMaxLength(100);
            b.Property(t => t.SuspendedReason).HasMaxLength(500);
            b.Property(t => t.Remark).HasMaxLength(1000);
            b.Property(t => t.TaskProgress).HasColumnType("decimal(5,2)");
            b.Property(t => t.TaskType).HasConversion(e => e.Value, v => Wms.Shared.Domain.Enums.TaskType.FromValue(v));
            b.Property(t => t.TaskPriority).HasConversion(e => e.Value, v => Wms.Shared.Domain.Enums.TaskPriority.FromValue(v));
            b.Property(t => t.TaskStatus).HasConversion(e => e.Value, v => Wms.TaskCenter.Domain.Enums.TaskStatus.FromValue(v));
            b.Property(t => t.AssignmentStrategy).HasConversion(e => e.Value, v => Wms.TaskCenter.Domain.Enums.AssignmentStrategy.FromValue(v));
            b.Property(t => t.AssignedUserId).IsRequired(false);
            b.HasIndex(t => t.TaskNo).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(t => new { t.WarehouseId, t.TaskStatus });
            b.HasIndex(t => new { t.AssignedUserId, t.TaskStatus });
            b.HasIndex(t => new { t.SourceOrderType, t.SourceOrderId });
            b.HasIndex(t => new { t.TaskPriority, t.TaskStatus });
            b.HasIndex(t => new { t.ExpectedCompletionTime, t.TaskStatus });
        });
    }

    /// <summary>
    /// Configures ExtraProperties value conversion for all WMS entities that implement IHasExtraProperties.
    /// ABP's base.OnModelCreating should handle this automatically, but for entities configured via
    /// IEntityTypeConfiguration or inline methods after base.OnModelCreating, the value conversion
    /// may not be applied. This method ensures all relevant entities have ExtraProperties
    /// mapped as a JSON value conversion column (nvarchar(max)).
    /// </summary>
    private static void ConfigureExtraPropertiesValueConversion(ModelBuilder builder)
    {
        // All WMS entities that implement IHasExtraProperties (inherit FullAuditedAggregateRoot or FullAuditedEntity)
        // Note: Entities inheriting Entity<Guid> only (ApprovalActionLog, ApprovalNode, InventoryLedgerEntry)
        // do NOT implement IHasExtraProperties and are excluded.
        var wmsEntityTypes = new[]
        {
            // Warehouse module (FullAuditedAggregateRoot)
            typeof(Wms.Warehouse.Domain.Aggregates.Warehouse),
            typeof(Wms.Warehouse.Domain.Aggregates.WarehouseArea),
            typeof(Wms.Warehouse.Domain.Aggregates.Location),
            // Material module
            typeof(Wms.Material.Domain.Aggregates.Material),           // FullAuditedAggregateRoot
            typeof(Wms.Material.Domain.Aggregates.MaterialClassification), // FullAuditedAggregateRoot
            typeof(Wms.Material.Domain.Aggregates.MaterialIssueStrategy), // AuditedAggregateRoot
            typeof(Wms.Material.Domain.Entities.UnitOfMeasure),         // FullAuditedEntity (in Entities namespace)
            typeof(Wms.Material.Domain.Aggregates.MaterialSubstituteRelation), // FullAuditedEntity
            // Inventory module
            typeof(Wms.Inventory.Domain.Aggregates.InventoryBalance),   // FullAuditedAggregateRoot
            typeof(Wms.Inventory.Domain.Aggregates.InventoryAdjustment), // FullAuditedAggregateRoot
            typeof(Wms.Inventory.Domain.Aggregates.InventoryAdjustmentLine), // FullAuditedEntity
            typeof(Wms.Inventory.Domain.Aggregates.InventoryFreezeOrder), // FullAuditedAggregateRoot
            typeof(Wms.Inventory.Domain.Aggregates.InventoryAlert),     // FullAuditedAggregateRoot
            // Inbound module
            typeof(Wms.Inbound.Domain.Aggregates.InboundOrder),         // FullAuditedAggregateRoot
            typeof(Wms.Inbound.Domain.Aggregates.InboundLine),          // FullAuditedEntity
            // Outbound module
            typeof(Wms.Outbound.Domain.Aggregates.OutboundOrder),       // FullAuditedAggregateRoot
            typeof(Wms.Outbound.Domain.Aggregates.OutboundLine),        // FullAuditedEntity
            // BarcodeLabel module (FullAuditedAggregateRoot)
            typeof(Wms.BarcodeLabel.Domain.Aggregates.BarcodeRule),
            typeof(Wms.BarcodeLabel.Domain.Aggregates.LabelTemplate),
            typeof(Wms.BarcodeLabel.Domain.Aggregates.PrintTask),
            // RuleEngine module (FullAuditedAggregateRoot)
            typeof(Wms.RuleEngine.Domain.Aggregates.BusinessRule),
            typeof(Wms.RuleEngine.Domain.Aggregates.IndustryPackage),
            // Notification module (FullAuditedAggregateRoot)
            typeof(Wms.Notification.Domain.Aggregates.Notification),
            typeof(Wms.Notification.Domain.Aggregates.NotificationTemplate),
            typeof(Wms.Notification.Domain.Aggregates.NotificationRule),
            // LineSide module
            typeof(Wms.LineSide.Domain.Aggregates.LineSideWarehouse),   // FullAuditedAggregateRoot
            typeof(Wms.LineSide.Domain.Aggregates.LineSideKanbanItem),  // FullAuditedEntity
            // Transfer module
            typeof(Wms.Transfer.Domain.Aggregates.TransferOrder),       // FullAuditedAggregateRoot
            typeof(Wms.Transfer.Domain.Aggregates.TransferLine),        // FullAuditedEntity
            // Production module
            typeof(Wms.Production.Domain.Aggregates.MaterialRequisition), // FullAuditedAggregateRoot
            typeof(Wms.Production.Domain.Aggregates.MaterialRequisitionLine), // FullAuditedEntity
            typeof(Wms.Production.Domain.Aggregates.SubcontractOrder), // FullAuditedAggregateRoot
            // CycleCount module
            typeof(Wms.CycleCount.Domain.Aggregates.CycleCountPlan),    // FullAuditedAggregateRoot
            typeof(Wms.CycleCount.Domain.Aggregates.CycleCountItem),    // FullAuditedEntity
            typeof(Wms.CycleCount.Domain.Aggregates.CycleCountResult),  // FullAuditedAggregateRoot
            // Workflow module
            typeof(Wms.Workflow.Domain.Aggregates.ApprovalFlow),        // FullAuditedAggregateRoot
            typeof(Wms.Workflow.Domain.Aggregates.ApprovalInstance),    // FullAuditedAggregateRoot
            // TaskCenter module (FullAuditedAggregateRoot)
            typeof(Wms.TaskCenter.Domain.Aggregates.WarehouseTask),
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var comparer = new ValueComparer<ExtraPropertyDictionary>(
            (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) == JsonSerializer.Serialize(c2, jsonOptions),
            c => c == null ? 0 : JsonSerializer.Serialize(c, jsonOptions).GetHashCode(),
            c => JsonSerializer.Deserialize<ExtraPropertyDictionary>(JsonSerializer.Serialize(c, jsonOptions), jsonOptions)
        );

        foreach (var entityType in wmsEntityTypes)
        {
            builder.Entity(entityType, b =>
            {
                var property = b.Property<ExtraPropertyDictionary>("ExtraProperties")
                    .HasColumnType("nvarchar(max)")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, jsonOptions),
                        v => JsonSerializer.Deserialize<ExtraPropertyDictionary>(v, jsonOptions)
                    );

                property.Metadata.SetValueComparer(comparer);
            });
        }
    }
}