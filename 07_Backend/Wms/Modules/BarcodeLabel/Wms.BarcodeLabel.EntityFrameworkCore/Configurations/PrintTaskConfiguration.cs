using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.EntityFrameworkCore.Configurations;

/// <summary>
/// PrintTask EF Core Configuration — configures table, indexes, and property mappings.
/// Table: WmsPrintTasks
/// </summary>
public class PrintTaskConfiguration : IEntityTypeConfiguration<PrintTask>
{
    public void Configure(EntityTypeBuilder<PrintTask> builder)
    {
        builder.ToTable("WmsPrintTasks");
        builder.HasKey(e => e.Id);

        // Index on TaskNo
        builder.HasIndex(e => e.TaskNo)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_PrintTask_TaskNo");

        // Index on PrintStatus
        builder.HasIndex(e => e.PrintStatus)
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_PrintTask_Status");

        // Composite index on SourceOrderType + SourceOrderId
        builder.HasIndex(e => new { e.SourceOrderType, e.SourceOrderId })
            .HasFilter("[IsDeleted] = 0")
            .HasName("IDX_BL_PrintTask_SourceOrder");

        // Index on PrinterId
        builder.HasIndex(e => e.PrinterId)
            .HasFilter("[IsDeleted] = 0 AND [PrinterId] IS NOT NULL")
            .HasName("IDX_BL_PrintTask_Printer");

        // Property configurations
        builder.Property(e => e.TaskNo).IsRequired().HasMaxLength(50);
        builder.Property(e => e.PrinterId).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.PrinterName).HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.TemplateId).IsRequired();
        builder.Property(e => e.TemplateName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.SourceOrderType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.SourceOrderId).IsRequired();
        builder.Property(e => e.PrintContent).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(e => e.PrintQuantity).IsRequired();
        builder.Property(e => e.RetryCount).IsRequired().HasDefaultValue(0);
        builder.Property(e => e.MaxRetryCount).IsRequired().HasDefaultValue(3);
        builder.Property(e => e.ErrorMessage).HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.CompletedTime).IsRequired(false);

        // SmartEnum value conversion: PrintTaskStatus stored as int
        builder.Property(e => e.PrintStatus)
            .HasConversion(
                v => v.Value,
                v => PrintTaskStatus.FromValue(v))
            .IsRequired();
    }
}
