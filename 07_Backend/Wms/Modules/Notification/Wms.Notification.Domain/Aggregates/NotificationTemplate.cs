using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.Notification.Domain.Enums;

namespace Wms.Notification.Domain.Aggregates;

/// <summary>
/// NotificationTemplate Aggregate Root — AGG-29
/// Template with {variable} placeholders for notifications.
/// </summary>
public class NotificationTemplate : FullAuditedAggregateRoot<Guid>
{
    public string TemplateName { get; private set; }
    public NotificationType TemplateType { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string TemplateContent { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    private NotificationTemplate() { }

    public NotificationTemplate(
        Guid id,
        string templateName,
        NotificationType templateType,
        NotificationChannel channel,
        string templateContent,
        string? description = null)
        : base(id)
    {
        TemplateName = Check.NotNullOrWhiteSpace(templateName, nameof(templateName), maxLength: 100);
        TemplateType = templateType ?? throw new ArgumentNullException(nameof(templateType));
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        TemplateContent = Check.NotNullOrWhiteSpace(templateContent, nameof(templateContent));
        IsActive = true;
        Description = description;
    }

    public void UpdateContent(string newContent)
    {
        TemplateContent = Check.NotNullOrWhiteSpace(newContent, nameof(newContent));
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Render template by replacing {variable} placeholders with provided values.
    /// </summary>
    public string RenderTemplate(Dictionary<string, string> variables)
    {
        if (variables == null)
            throw new ArgumentNullException(nameof(variables));

        var result = TemplateContent;
        foreach (var kvp in variables)
        {
            result = result.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        return result;
    }
}
