using Volo.Abp.Application.Dtos;

namespace Wms.Notification.Application.Contracts.Dtos;

public class NotificationRuleCreateDto
{
    public string RuleName { get; set; } = string.Empty;
    public string SourceEvent { get; set; } = string.Empty;
    public string SourceModule { get; set; } = string.Empty;
    public string? TargetRole { get; set; }
    public int TargetChannelValue { get; set; }
    public int NotificationTypeValue { get; set; }
    public Guid? TemplateId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
    public int PriorityValue { get; set; } = 1;
}

public class NotificationRuleOutputDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string SourceEvent { get; set; } = string.Empty;
    public string SourceModule { get; set; } = string.Empty;
    public string? TargetRole { get; set; }
    public int TargetChannelValue { get; set; }
    public int NotificationTypeValue { get; set; }
    public Guid? TemplateId { get; set; }
    public bool IsEnabled { get; set; }
    public string? Description { get; set; }
    public int PriorityValue { get; set; }
}

public class NotificationRuleQueryDto : PagedResultRequestDto
{
    public string? SourceEvent { get; set; }
    public bool? IsEnabled { get; set; }
}
