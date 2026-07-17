using Xunit;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Tests.Domain;

/// <summary>
/// Notification Domain Tests — 5 tests for aggregate root methods.
/// </summary>
public class NotificationDomainTests
{
    private static NotificationEntity CreatePendingNotification()
    {
        return new NotificationEntity(
            Guid.NewGuid(),
            NotificationType.Alert,
            NotificationChannel.Internal,
            "Test Title",
            "Test Content",
            Guid.NewGuid(),
            "Test User",
            NotificationPriority.Normal,
            "TestEvent",
            "TestModule");
    }

    [Fact]
    public void MarkAsSent_ShouldSetSendStatusToSent()
    {
        // Arrange
        var notification = CreatePendingNotification();

        // Act
        notification.MarkAsSent();

        // Assert
        Assert.Equal(SendStatus.Sent, notification.SendStatus);
        Assert.NotNull(notification.SendTime);
    }

    [Fact]
    public void MarkAsSent_WhenAlreadySent_ShouldThrowException()
    {
        // Arrange
        var notification = CreatePendingNotification();
        notification.MarkAsSent();

        // Act & Assert
        var ex = Assert.Throws<BusinessException>(() => notification.MarkAsSent());
        Assert.Contains("WMS:Notification", ex.Code);
    }

    [Fact]
    public void MarkAsFailed_ShouldSetFailedStatusAndErrorMessage()
    {
        // Arrange
        var notification = CreatePendingNotification();
        const string error = "Channel unavailable";

        // Act
        notification.MarkAsFailed(error);

        // Assert
        Assert.Equal(SendStatus.Failed, notification.SendStatus);
        Assert.Equal(error, notification.ErrorMessage);
        Assert.Equal(1, notification.RetryCount);
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadStatusAndTime()
    {
        // Arrange
        var notification = CreatePendingNotification();

        // Act
        notification.MarkAsRead();

        // Assert
        Assert.Equal(ReadStatus.Read, notification.ReadStatus);
        Assert.NotNull(notification.ReadTime);
    }

    [Fact]
    public void Retry_ShouldTransitionFromFailedToRetrying()
    {
        // Arrange
        var notification = CreatePendingNotification();
        notification.MarkAsFailed("Initial failure");

        // Act
        notification.Retry();

        // Assert
        Assert.Equal(SendStatus.Retrying, notification.SendStatus);
        Assert.Null(notification.ErrorMessage);
    }

    [Fact]
    public void MarkAsRead_WhenAlreadyRead_ShouldThrowException()
    {
        // Arrange
        var notification = CreatePendingNotification();
        notification.MarkAsRead();

        // Act & Assert
        var ex = Assert.Throws<BusinessException>(() => notification.MarkAsRead());
        Assert.Contains("WMS:Notification", ex.Code);
    }
}
