using AutoMapper;
using Wms.CycleCount.Application.Contracts.Dtos;
using Wms.CycleCount.Domain.Aggregates;

namespace Wms.CycleCount.Application.Mappings;

public class CycleCountAutoMapperProfile : Profile
{
    public CycleCountAutoMapperProfile()
    {
        CreateMap<CycleCountPlan, CycleCountPlanOutputDto>()
            .ForMember(d => d.CountMethodDescription, opt => opt.MapFrom(s => s.CountMethod.Description))
            .ForMember(d => d.CountStatusDescription, opt => opt.MapFrom(s => s.CountStatus.Description));
        CreateMap<CycleCountItem, CycleCountItemOutputDto>();
    }
}
