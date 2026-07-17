using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Wms.Notification.Application.Contracts.Dtos;

namespace Wms.Notification.Application.Contracts.Services;

/// <summary>
/// INotificationAppService — 8 API methods (API-NT-001~008)
/// </summary>
public interface INotificationAppService : IApplicationService
{
    // API-NT-001: Get template list
    Task<PagedResultDto<NotificationTemplateOutputDto>> GetTemplateListAsync(NotificationTemplateQueryDto input);

    // API-NT-002: Get template detail
    Task<NotificationTemplateOutputDto> GetTemplateAsync(Guid id);

    // API-NT-003: Create template
    Task<NotificationTemplateOutputDto> CreateTemplateAsync(NotificationTemplateCreateDto input);

    // API-NT-004: Get notification log list
    Task<PagedResultDto<NotificationLogOutputDto>> GetLogListAsync(NotificationLogQueryDto input);

    // API-NT-005: Get my notifications
    Task<PagedResultDto<MyNotificationOutputDto>> GetMyNotificationsAsync(int skipCount, int maxResultCount);

    // API-NT-006: Mark as read
    Task MarkAsReadAsync(Guid id);

    // API-NT-007: Get rule list
    Task<PagedResultDto<NotificationRuleOutputDto>> GetRuleListAsync(NotificationRuleQueryDto input);

    // API-NT-008: Create rule
    Task<NotificationRuleOutputDto> CreateRuleAsync(NotificationRuleCreateDto input);
}
