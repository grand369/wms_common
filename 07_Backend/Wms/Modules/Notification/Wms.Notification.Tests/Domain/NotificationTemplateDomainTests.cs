using Xunit;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Tests.Domain;

/// <summary>
/// NotificationTemplate Domain Tests — 4 tests for template methods.
/// </summary>
public class NotificationTemplateDomainTests
{
    private static NotificationTemplate CreateTemplate()
    {
        return new NotificationTemplate(
            Guid.NewGuid(),
            "AlertTemplate",
            NotificationType.Alert,
            NotificationChannel.Internal,
            "物料 {MaterialCode} 库存低于安全库存 {SafetyStock}，当前可用 {CurrentQty}");
    }

    [Fact]
    public void RenderTemplate_ShouldReplaceAllVariables()
    {
        // Arrange
        var template = CreateTemplate();
        var variables = new Dictionary<string, string>
        {
            { "MaterialCode", "M001" },
            { "SafetyStock", "100" },
            { "CurrentQty", "50" }
        };

        // Act
        var result = template.RenderTemplate(variables);

        // Assert
        Assert.Contains("M001", result);
        Assert.Contains("100", result);
        Assert.Contains("50", result);
        Assert.DoesNotContain("{", result);
    }

    [Fact]
    public void UpdateContent_ShouldChangeTemplateContent()
    {
        // Arrange
        var template = CreateTemplate();
        const string newContent = "New template content {var1}";

        // Act
        template.UpdateContent(newContent);

        // Assert
        Assert.Equal(newContent, template.TemplateContent);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var template = CreateTemplate();
        template.Deactivate();
        Assert.False(template.IsActive);

        // Act
        template.Activate();

        // Assert
        Assert.True(template.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var template = CreateTemplate();
        Assert.True(template.IsActive);

        // Act
        template.Deactivate();

        // Assert
        Assert.False(template.IsActive);
    }
}
