using AutoMapper;
using Wms.LineSide.Application.Contracts.Dtos;
using Wms.LineSide.Domain.Aggregates;

namespace Wms.LineSide.Application.Mappings;

public class LineSideAutoMapperProfile : Profile
{
    public LineSideAutoMapperProfile()
    {
        CreateMap<LineSideWarehouse, LineSideWarehouseOutputDto>()
            .ForMember(d => d.ConsumptionModeDescription, opt => opt.MapFrom(s => s.ConsumptionMode.Description));
        CreateMap<LineSideKanbanItem, LineSideKanbanItemOutputDto>();
    }
}
