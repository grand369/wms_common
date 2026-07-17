using Volo.Abp.Application.Dtos;

namespace Wms.Notification.Application.Contracts.Dtos;

public class NotificationLogOutputDto
{
    public Guid Id { get; set; }
    public int NotificationTypeValue { get; set; }
    public int ChannelValue { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public int SendStatusValue { get; set; }
    public DateTime? SendTime { get; set; }
    public int ReadStatusValue { get; set; }
    public DateTime? ReadTime { get; set; }
    public int PriorityValue { get; set; }
    public string? SourceEvent { get; set; }
    public string? SourceModule { get; set; }
    public Guid? CorrelationId { get; set; }
}

public class NotificationLogQueryDto : PagedResultRequestDto
{
    public int? NotificationTypeValue { get; set; }
    public int? ChannelValue { get; set; }
    public int? SendStatusValue { get; set; }
    public int? ReadStatusValue { get; set; }
    public Guid? RecipientId { get; set; }
}

public class MyNotificationOutputDto
{
    public Guid Id { get; set; }
    public int NotificationTypeValue { get; set; }
    public int ChannelValue { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? SendTime { get; set; }
    public int ReadStatusValue { get; set; }
    public int PriorityValue { get; set; }
}
