using System;
using FluentAssertions;
using Volo.Abp;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Xunit;

namespace Wms.BarcodeLabel.Tests.Domain;

public class PrintTaskDomainTests
{
    private PrintTask CreateSampleTask(PrintTaskStatus initialStatus)
    {
        var task = new PrintTask(
            Guid.NewGuid(),
            "PRT-250101-0001",
            "Inbound",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "<label>test</label>",
            1);

        // Set status directly for test scenarios
        if (initialStatus == PrintTaskStatus.Printing)
        {
            task.MarkPrinting();
        }
        else if (initialStatus == PrintTaskStatus.Failed)
        {
            task.MarkPrinting();
            task.MarkFailed("Test error");
        }
        else if (initialStatus == PrintTaskStatus.Completed)
        {
            task.MarkPrinting();
            task.MarkCompleted();
        }

        return task;
    }

    [Fact]
    public void NewPrintTask_StartsAsPending()
    {
        var task = CreateSampleTask(PrintTaskStatus.Pending);
        task.PrintStatus.Should().Be(PrintTaskStatus.Pending);
    }

    [Fact]
    public void MarkPrinting_FromPending_Succeeds()
    {
        var task = CreateSampleTask(PrintTaskStatus.Pending);
        task.MarkPrinting();
        task.PrintStatus.Should().Be(PrintTaskStatus.Printing);
    }

    [Fact]
    public void MarkCompleted_FromPrinting_Succeeds()
    {
        var task = CreateSampleTask(PrintTaskStatus.Printing);
        task.MarkCompleted();
        task.PrintStatus.Should().Be(PrintTaskStatus.Completed);
        task.CompletedTime.Should().NotBeNull();
    }

    [Fact]
    public void MarkFailed_FromPrinting_Succeeds()
    {
        var task = CreateSampleTask(PrintTaskStatus.Printing);
        task.MarkFailed("Printer offline");
        task.PrintStatus.Should().Be(PrintTaskStatus.Failed);
        task.ErrorMessage.Should().Be("Printer offline");
    }

    [Fact]
    public void Retry_FromFailed_Succeeds()
    {
        var task = CreateSampleTask(PrintTaskStatus.Failed);
        task.Retry();
        task.PrintStatus.Should().Be(PrintTaskStatus.Pending);
        task.RetryCount.Should().Be(1);
        task.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Retry_ExceedingMaxRetry_ThrowsBusinessException()
    {
        var task = new PrintTask(
            Guid.NewGuid(),
            "PRT-250101-0002",
            "Inbound",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "<label>test</label>",
            1,
            maxRetryCount: 1);

        task.MarkPrinting();
        task.MarkFailed("Error");
        task.Retry(); // Retry 1 of 1
        task.MarkPrinting();
        task.MarkFailed("Error again");

        var act = () => task.Retry();
        act.Should().Throw<BusinessException>()
            .WithMessage("*maximum retry count*");
    }

    [Fact]
    public void Cancel_FromNonPending_Succeeds()
    {
        var task = CreateSampleTask(PrintTaskStatus.Failed);
        task.Cancel();
        task.PrintStatus.Should().Be(PrintTaskStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromCompleted_ThrowsBusinessException()
    {
        var task = CreateSampleTask(PrintTaskStatus.Completed);

        var act = () => task.Cancel();
        act.Should().Throw<BusinessException>()
            .WithMessage("*Cannot cancel a completed*");
    }
}
