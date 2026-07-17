using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Wms.Notification.Application.Contracts.Dtos;
using Wms.Notification.Application.Contracts.Permissions;
using Wms.Notification.Application.Contracts.Services;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Repositories;
using Wms.Notification.Domain.Services;

namespace Wms.Notification.Application.Services;

/// <summary>
/// NotificationAppService — 8 API methods (API-NT-001~008)
/// </summary>
[Authorize(WmsNotificationPermissions.Read)]
public class NotificationAppService : ApplicationService, INotificationAppService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly INotificationRuleRepository _ruleRepository;
    private readonly NotificationDomainService _notificationDomainService;

    public NotificationAppService(
        INotificationRepository notificationRepository,
        INotificationTemplateRepository templateRepository,
        INotificationRuleRepository ruleRepository,
        NotificationDomainService notificationDomainService)
    {
        _notificationRepository = notificationRepository;
        _templateRepository = templateRepository;
        _ruleRepository = ruleRepository;
        _notificationDomainService = notificationDomainService;
    }

    // ── Template ──

    public async Task<PagedResultDto<NotificationTemplateOutputDto>> GetTemplateListAsync(NotificationTemplateQueryDto input)
    {
        var query = await _templateRepository.GetQueryableAsync();

        if (input.TemplateTypeValue.HasValue)
            query = query.Where(t => t.TemplateType.Value == input.TemplateTypeValue.Value);
        if (input.ChannelValue.HasValue)
            query = query.Where(t => t.Channel.Value == input.ChannelValue.Value);
        if (input.IsActive.HasValue)
            query = query.Where(t => t.IsActive == input.IsActive.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderBy(t => t.TemplateName).Skip(input.SkipCount).Take(input.MaxResultCount);
        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<NotificationTemplateOutputDto>(
            totalCount,
            items.Select(MapToTemplateOutputDto).ToList());
    }

    public async Task<NotificationTemplateOutputDto> GetTemplateAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        return MapToTemplateOutputDto(template);
    }

    [Authorize(WmsNotificationPermissions.Create)]
    public async Task<NotificationTemplateOutputDto> CreateTemplateAsync(NotificationTemplateCreateDto input)
    {
        var templateType = NotificationType.FromValue(input.TemplateTypeValue);
        var channel = NotificationChannel.FromValue(input.ChannelValue);

        var template = new NotificationTemplate(
            GuidGenerator.Create(),
            input.TemplateName,
            templateType,
            channel,
            input.TemplateContent,
            input.Description);

        await _templateRepository.InsertAsync(template);
        return MapToTemplateOutputDto(template);
    }

    // ── Log ──

    public async Task<PagedResultDto<NotificationLogOutputDto>> GetLogListAsync(NotificationLogQueryDto input)
    {
        var query = await _notificationRepository.GetQueryableAsync();

        if (input.NotificationTypeValue.HasValue)
            query = query.Where(n => n.NotificationType.Value == input.NotificationTypeValue.Value);
        if (input.ChannelValue.HasValue)
            query = query.Where(n => n.Channel.Value == input.ChannelValue.Value);
        if (input.SendStatusValue.HasValue)
            query = query.Where(n => n.SendStatus.Value == input.SendStatusValue.Value);
        if (input.ReadStatusValue.HasValue)
            query = query.Where(n => n.ReadStatus.Value == input.ReadStatusValue.Value);
        if (input.RecipientId.HasValue)
            query = query.Where(n => n.RecipientId == input.RecipientId.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderByDescending(n => n.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount);
        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<NotificationLogOutputDto>(
            totalCount,
            items.Select(MapToLogOutputDto).ToList());
    }

    public async Task<PagedResultDto<MyNotificationOutputDto>> GetMyNotificationsAsync(int skipCount, int maxResultCount)
    {
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var notifications = await _notificationRepository.GetByRecipientAsync(currentUserId);

        var filtered = notifications.AsQueryable()
            .OrderByDescending(n => n.CreationTime);

        var totalCount = filtered.Count();
        var items = filtered
            .Skip(skipCount)
            .Take(maxResultCount)
            .Select(MapToMyNotificationOutputDto)
            .ToList();

        return new PagedResultDto<MyNotificationOutputDto>(totalCount, items);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        await _notificationDomainService.MarkAsReadBulkAsync(currentUserId, new List<Guid> { id });
    }

    // ── Rule ──

    public async Task<PagedResultDto<NotificationRuleOutputDto>> GetRuleListAsync(NotificationRuleQueryDto input)
    {
        var query = await _ruleRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.SourceEvent))
            query = query.Where(r => r.SourceEvent.Contains(input.SourceEvent));
        if (input.IsEnabled.HasValue)
            query = query.Where(r => r.IsEnabled == input.IsEnabled.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderBy(r => r.RuleName).Skip(input.SkipCount).Take(input.MaxResultCount);
        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<NotificationRuleOutputDto>(
            totalCount,
            items.Select(MapToRuleOutputDto).ToList());
    }

    [Authorize(WmsNotificationPermissions.Create)]
    public async Task<NotificationRuleOutputDto> CreateRuleAsync(NotificationRuleCreateDto input)
    {
        var channel = NotificationChannel.FromValue(input.TargetChannelValue);
        var notificationType = NotificationType.FromValue(input.NotificationTypeValue);
        var priority = NotificationPriority.FromValue(input.PriorityValue);

        var rule = new NotificationRule(
            GuidGenerator.Create(),
            input.RuleName,
            input.SourceEvent,
            input.SourceModule,
            channel,
            notificationType,
            priority,
            input.TargetRole,
            input.TemplateId,
            input.IsEnabled,
            input.Description);

        await _ruleRepository.InsertAsync(rule);
        return MapToRuleOutputDto(rule);
    }

    // ── Mapping helpers ──

    private static NotificationTemplateOutputDto MapToTemplateOutputDto(NotificationTemplate t)
    {
        return new NotificationTemplateOutputDto
        {
            Id = t.Id,
            TemplateName = t.TemplateName,
            TemplateTypeValue = t.TemplateType.Value,
            ChannelValue = t.Channel.Value,
            TemplateContent = t.TemplateContent,
            IsActive = t.IsActive,
            Description = t.Description
        };
    }

    private static NotificationLogOutputDto MapToLogOutputDto(NotificationEntity n)
    {
        return new NotificationLogOutputDto
        {
            Id = n.Id,
            NotificationTypeValue = n.NotificationType.Value,
            ChannelValue = n.Channel.Value,
            Title = n.Title,
            Content = n.Content,
            RecipientId = n.RecipientId,
            RecipientName = n.RecipientName,
            SendStatusValue = n.SendStatus.Value,
            SendTime = n.SendTime,
            ReadStatusValue = n.ReadStatus.Value,
            ReadTime = n.ReadTime,
            PriorityValue = n.Priority.Value,
            SourceEvent = n.SourceEvent,
            SourceModule = n.SourceModule,
            CorrelationId = n.CorrelationId
        };
    }

    private static MyNotificationOutputDto MapToMyNotificationOutputDto(NotificationEntity n)
    {
        return new MyNotificationOutputDto
        {
            Id = n.Id,
            NotificationTypeValue = n.NotificationType.Value,
            ChannelValue = n.Channel.Value,
            Title = n.Title,
            Content = n.Content?.Length > 100 ? n.Content[..100] + "..." : n.Content ?? string.Empty,
            SendTime = n.SendTime,
            ReadStatusValue = n.ReadStatus.Value,
            PriorityValue = n.Priority.Value
        };
    }

    private static NotificationRuleOutputDto MapToRuleOutputDto(NotificationRule r)
    {
        return new NotificationRuleOutputDto
        {
            Id = r.Id,
            RuleName = r.RuleName,
            SourceEvent = r.SourceEvent,
            SourceModule = r.SourceModule,
            TargetRole = r.TargetRole,
            TargetChannelValue = r.TargetChannel.Value,
            NotificationTypeValue = r.NotificationType.Value,
            TemplateId = r.TemplateId,
            IsEnabled = r.IsEnabled,
            Description = r.Description,
            PriorityValue = r.Priority.Value
        };
    }
}
