using AutoMapper;
using Wms.Notification.Application.Contracts.Dtos;
using Wms.Notification.Domain.Aggregates;
using NotificationEntity = Wms.Notification.Domain.Aggregates.Notification;

namespace Wms.Notification.Application.Mappings;

/// <summary>
/// AutoMapper profile for Notification module — SmartEnum → Value mapping.
/// </summary>
public class NotificationAutoMapperProfile : Profile
{
    public NotificationAutoMapperProfile()
    {
        CreateMap<NotificationEntity, NotificationLogOutputDto>()
            .ForMember(d => d.NotificationTypeValue, opt => opt.MapFrom(s => s.NotificationType.Value))
            .ForMember(d => d.ChannelValue, opt => opt.MapFrom(s => s.Channel.Value))
            .ForMember(d => d.SendStatusValue, opt => opt.MapFrom(s => s.SendStatus.Value))
            .ForMember(d => d.ReadStatusValue, opt => opt.MapFrom(s => s.ReadStatus.Value))
            .ForMember(d => d.PriorityValue, opt => opt.MapFrom(s => s.Priority.Value));

        CreateMap<NotificationEntity, MyNotificationOutputDto>()
            .ForMember(d => d.NotificationTypeValue, opt => opt.MapFrom(s => s.NotificationType.Value))
            .ForMember(d => d.ChannelValue, opt => opt.MapFrom(s => s.Channel.Value))
            .ForMember(d => d.ReadStatusValue, opt => opt.MapFrom(s => s.ReadStatus.Value))
            .ForMember(d => d.PriorityValue, opt => opt.MapFrom(s => s.Priority.Value));

        CreateMap<NotificationTemplate, NotificationTemplateOutputDto>()
            .ForMember(d => d.TemplateTypeValue, opt => opt.MapFrom(s => s.TemplateType.Value))
            .ForMember(d => d.ChannelValue, opt => opt.MapFrom(s => s.Channel.Value));

        CreateMap<NotificationRule, NotificationRuleOutputDto>()
            .ForMember(d => d.TargetChannelValue, opt => opt.MapFrom(s => s.TargetChannel.Value))
            .ForMember(d => d.NotificationTypeValue, opt => opt.MapFrom(s => s.NotificationType.Value))
            .ForMember(d => d.PriorityValue, opt => opt.MapFrom(s => s.Priority.Value));
    }
}
