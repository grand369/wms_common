using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Notification.Application.Contracts.Dtos;
using Wms.Notification.Application.Contracts.Services;

namespace Wms.Notification.HttpApi.Controllers;

/// <summary>
/// Notification Controller — REST endpoints under /api/v1/notification/
/// 8 API endpoints (API-NT-001~008)
/// </summary>
[Route("api/v1/notification")]
[Authorize]
public class NotificationController : AbpControllerBase
{
    private readonly INotificationAppService _appService;

    public NotificationController(INotificationAppService appService)
    {
        _appService = appService;
    }

    // API-NT-003: Create template
    [HttpPost("templates")]
    public Task<NotificationTemplateOutputDto> CreateTemplateAsync(NotificationTemplateCreateDto input)
    {
        return _appService.CreateTemplateAsync(input);
    }

    // API-NT-001: Get template list
    [HttpGet("templates")]
    public Task<PagedResultDto<NotificationTemplateOutputDto>> GetTemplateListAsync(NotificationTemplateQueryDto input)
    {
        return _appService.GetTemplateListAsync(input);
    }

    // API-NT-002: Get template detail
    [HttpGet("templates/{id}")]
    public Task<NotificationTemplateOutputDto> GetTemplateAsync(Guid id)
    {
        return _appService.GetTemplateAsync(id);
    }

    // API-NT-004: Get notification log list
    [HttpGet("logs")]
    public Task<PagedResultDto<NotificationLogOutputDto>> GetLogListAsync(NotificationLogQueryDto input)
    {
        return _appService.GetLogListAsync(input);
    }

    // API-NT-005: Get my notifications
    [HttpGet("logs/my")]
    public Task<PagedResultDto<MyNotificationOutputDto>> GetMyNotificationsAsync(int skipCount = 0, int maxResultCount = 20)
    {
        return _appService.GetMyNotificationsAsync(skipCount, maxResultCount);
    }

    // API-NT-006: Mark as read
    [HttpPatch("logs/{id}/mark-read")]
    public Task MarkAsReadAsync(Guid id)
    {
        return _appService.MarkAsReadAsync(id);
    }

    // API-NT-007: Get rule list
    [HttpGet("rules")]
    public Task<PagedResultDto<NotificationRuleOutputDto>> GetRuleListAsync(NotificationRuleQueryDto input)
    {
        return _appService.GetRuleListAsync(input);
    }

    // API-NT-008: Create rule
    [HttpPost("rules")]
    public Task<NotificationRuleOutputDto> CreateRuleAsync(NotificationRuleCreateDto input)
    {
        return _appService.CreateRuleAsync(input);
    }
}
