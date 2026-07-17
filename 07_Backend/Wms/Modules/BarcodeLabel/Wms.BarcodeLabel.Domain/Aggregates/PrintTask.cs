using System;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.BarcodeLabel.Domain.Enums;
using Wms.BarcodeLabel.Domain.Events;

namespace Wms.BarcodeLabel.Domain.Aggregates;

/// <summary>
/// PrintTask Aggregate Root (AGG-23) — represents a print job request.
/// </summary>
public class PrintTask : FullAuditedAggregateRoot<Guid>
{
    public string TaskNo { get; private set; }
    public string? PrinterId { get; private set; }
    public string? PrinterName { get; private set; }
    public Guid TemplateId { get; private set; }
    public string? TemplateName { get; private set; }
    public string SourceOrderType { get; private set; }
    public Guid SourceOrderId { get; private set; }
    public string PrintContent { get; private set; }
    public int PrintQuantity { get; private set; }
    public PrintTaskStatus PrintStatus { get; private set; }
    public int RetryCount { get; private set; }
    public int MaxRetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime? CompletedTime { get; private set; }

    private PrintTask() { }

    public PrintTask(
        Guid id,
        string taskNo,
        string sourceOrderType,
        Guid sourceOrderId,
        Guid templateId,
        string printContent,
        int printQuantity,
        string? templateName = null,
        string? printerId = null,
        string? printerName = null,
        int maxRetryCount = 3)
        : base(id)
    {
        TaskNo = taskNo ?? throw new ArgumentNullException(nameof(taskNo));
        SourceOrderType = sourceOrderType ?? throw new ArgumentNullException(nameof(sourceOrderType));
        SourceOrderId = sourceOrderId;
        TemplateId = templateId;
        PrintContent = printContent ?? throw new ArgumentNullException(nameof(printContent));
        PrintQuantity = printQuantity > 0 ? printQuantity : throw new ArgumentException("PrintQuantity must be greater than 0.", nameof(printQuantity));
        TemplateName = templateName;
        PrinterId = printerId;
        PrinterName = printerName;
        MaxRetryCount = maxRetryCount;
        PrintStatus = PrintTaskStatus.Pending;
        RetryCount = 0;
    }

    /// <summary>Mark the print task as printing.</summary>
    public void MarkPrinting()
    {
        if (PrintStatus != PrintTaskStatus.Pending)
            throw new BusinessException("WMS:BarcodeLabel:InvalidStatusTransition",
                $"Cannot start printing when task status is {PrintStatus.Name}. Only Pending tasks can be started.");

        PrintStatus = PrintTaskStatus.Printing;
    }

    /// <summary>Mark the print task as completed.</summary>
    public void MarkCompleted()
    {
        if (PrintStatus != PrintTaskStatus.Printing)
            throw new BusinessException("WMS:BarcodeLabel:InvalidStatusTransition",
                $"Cannot complete when task status is {PrintStatus.Name}. Only Printing tasks can be completed.");

        PrintStatus = PrintTaskStatus.Completed;
        CompletedTime = DateTime.UtcNow;

        AddLocalEvent(new PrintCompletedEvent
        {
            AggregateRootId = Id,
            PrintTaskId = Id,
            TaskNo = TaskNo,
            PrinterId = PrinterId,
            SourceModule = "BarcodeLabel"
        });
    }

    /// <summary>Mark the print task as failed.</summary>
    public void MarkFailed(string errorMessage)
    {
        if (PrintStatus != PrintTaskStatus.Printing && PrintStatus != PrintTaskStatus.Pending)
            throw new BusinessException("WMS:BarcodeLabel:InvalidStatusTransition",
                $"Cannot mark as failed when task status is {PrintStatus.Name}.");

        PrintStatus = PrintTaskStatus.Failed;
        ErrorMessage = errorMessage;

        AddLocalEvent(new PrintFailedEvent
        {
            AggregateRootId = Id,
            PrintTaskId = Id,
            TaskNo = TaskNo,
            ErrorMessage = errorMessage,
            SourceModule = "BarcodeLabel"
        });
    }

    /// <summary>Retry the failed print task.</summary>
    public void Retry()
    {
        if (PrintStatus != PrintTaskStatus.Failed)
            throw new BusinessException("WMS:BarcodeLabel:InvalidStatusTransition",
                $"Cannot retry when task status is {PrintStatus.Name}. Only Failed tasks can be retried.");

        if (RetryCount >= MaxRetryCount)
            throw new BusinessException("WMS:BarcodeLabel:MaxRetryExceeded",
                $"Print task '{TaskNo}' has reached the maximum retry count ({MaxRetryCount}).");

        RetryCount++;
        PrintStatus = PrintTaskStatus.Pending;
        ErrorMessage = null;
    }

    /// <summary>Cancel the print task.</summary>
    public void Cancel()
    {
        if (PrintStatus == PrintTaskStatus.Completed)
            throw new BusinessException("WMS:BarcodeLabel:InvalidStatusTransition",
                "Cannot cancel a completed print task.");

        PrintStatus = PrintTaskStatus.Cancelled;
    }
}
