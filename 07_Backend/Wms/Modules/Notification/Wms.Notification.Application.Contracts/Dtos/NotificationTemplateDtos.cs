using Volo.Abp.Application.Dtos;

namespace Wms.Notification.Application.Contracts.Dtos;

public class NotificationTemplateCreateDto
{
    public string TemplateName { get; set; } = string.Empty;
    public int TemplateTypeValue { get; set; }
    public int ChannelValue { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class NotificationTemplateUpdateDto
{
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateContent { get; set; } = string.Empty;
    public int ChannelValue { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class NotificationTemplateOutputDto
{
    public Guid Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public int TemplateTypeValue { get; set; }
    public int ChannelValue { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class NotificationTemplateQueryDto : PagedResultRequestDto
{
    public int? TemplateTypeValue { get; set; }
    public int? ChannelValue { get; set; }
    public bool? IsActive { get; set; }
}
