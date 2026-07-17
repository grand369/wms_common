using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Application.Contracts.Dtos;
using AutoMapper;

namespace Wms.Outbound.Application.Mappings;

/// <summary>
/// Outbound Auto Mapper Profile — defines mappings between domain entities and DTOs.
/// Note: SmartEnum types are mapped to their int Value and string Description in DTOs.
/// </summary>
public class OutboundAutoMapperProfile : Profile
{
    public OutboundAutoMapperProfile()
    {
        CreateMap<OutboundOrder, OutboundOrderOutputDto>()
            .ForMember(d => d.OutboundTypeValue, opt => opt.MapFrom(s => s.OutboundType.Value))
            .ForMember(d => d.OutboundTypeName, opt => opt.MapFrom(s => s.OutboundType.Description))
            .ForMember(d => d.OutboundStatusValue, opt => opt.MapFrom(s => s.OutboundStatus.Value))
            .ForMember(d => d.OutboundStatusName, opt => opt.MapFrom(s => s.OutboundStatus.Description))
            .ForMember(d => d.ErpCallbackStatusValue, opt => opt.MapFrom(s => s.ErpCallbackStatus.Value))
            .ForMember(d => d.ErpCallbackStatusName, opt => opt.MapFrom(s => s.ErpCallbackStatus.Description));

        CreateMap<OutboundLine, OutboundLineOutputDto>()
            .ForMember(d => d.IssueStrategyValue, opt => opt.MapFrom(s => s.IssueStrategy.Value))
            .ForMember(d => d.IssueStrategyName, opt => opt.MapFrom(s => s.IssueStrategy.Description));
    }
}
