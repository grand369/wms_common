using Xunit;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Tests.Domain;

/// <summary>
/// NotificationRule Domain Tests — 3 tests for rule methods.
/// </summary>
public class NotificationRuleDomainTests
{
    private static NotificationRule CreateRule(bool isEnabled = true)
    {
        return new NotificationRule(
            Guid.NewGuid(),
            "SafetyStockRule",
            "SafetyStockAlert",
            "Inventory",
            NotificationChannel.Internal,
            NotificationType.Alert,
            NotificationPriority.High,
            null,
            null,
            isEnabled);
    }

    [Fact]
    public void Enable_ShouldSetIsEnabledToTrue()
    {
        // Arrange
        var rule = CreateRule(false);
        Assert.False(rule.IsEnabled);

        // Act
        rule.Enable();

        // Assert
        Assert.True(rule.IsEnabled);
    }

    [Fact]
    public void Disable_ShouldSetIsEnabledToFalse()
    {
        // Arrange
        var rule = CreateRule(true);
        Assert.True(rule.IsEnabled);

        // Act
        rule.Disable();

        // Assert
        Assert.False(rule.IsEnabled);
    }

    [Fact]
    public void UpdateTargetChannel_ShouldChangeChannel()
    {
        // Arrange
        var rule = CreateRule();
        Assert.Equal(NotificationChannel.Internal, rule.TargetChannel);

        // Act
        rule.UpdateTargetChannel(NotificationChannel.Email);

        // Assert
        Assert.Equal(NotificationChannel.Email, rule.TargetChannel);
    }

    [Fact]
    public void UpdateTemplate_ShouldChangeTemplateId()
    {
        // Arrange
        var rule = CreateRule();
        Assert.Null(rule.TemplateId);
        var newTemplateId = Guid.NewGuid();

        // Act
        rule.UpdateTemplate(newTemplateId);

        // Assert
        Assert.Equal(newTemplateId, rule.TemplateId);
    }
}
