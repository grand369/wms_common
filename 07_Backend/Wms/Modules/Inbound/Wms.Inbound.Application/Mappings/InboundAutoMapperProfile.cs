using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Application.Contracts.Dtos;
using AutoMapper;

namespace Wms.Inbound.Application.Mappings;

/// <summary>
/// Inbound Auto Mapper Profile — defines mappings between domain entities and DTOs.
/// Note: SmartEnum types are mapped to their int Value and string Description/Name in DTOs.
/// </summary>
public class InboundAutoMapperProfile : Profile
{
    public InboundAutoMapperProfile()
    {
        CreateMap<InboundOrder, InboundOrderOutputDto>()
            .ForMember(d => d.InboundTypeValue, opt => opt.MapFrom(s => s.InboundType.Value))
            .ForMember(d => d.InboundTypeName, opt => opt.MapFrom(s => s.InboundType.Description))
            .ForMember(d => d.InboundStatusValue, opt => opt.MapFrom(s => s.InboundStatus.Value))
            .ForMember(d => d.InboundStatusName, opt => opt.MapFrom(s => s.InboundStatus.Description))
            .ForMember(d => d.ErpCallbackStatusValue, opt => opt.MapFrom(s => s.ErpCallbackStatus.Value))
            .ForMember(d => d.ErpCallbackStatusName, opt => opt.MapFrom(s => s.ErpCallbackStatus.Description));

        CreateMap<InboundLine, InboundLineOutputDto>()
            .ForMember(d => d.QualityStatusValue, opt => opt.MapFrom(s => s.QualityStatus.Value))
            .ForMember(d => d.QualityStatusName, opt => opt.MapFrom(s => s.QualityStatus.Description));
    }
}
