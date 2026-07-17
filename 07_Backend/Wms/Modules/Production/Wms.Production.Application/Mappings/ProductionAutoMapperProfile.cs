using AutoMapper;
using Wms.Production.Application.Contracts.Dtos;
using Wms.Production.Domain.Aggregates;

namespace Wms.Production.Application.Mappings;

public class ProductionAutoMapperProfile : Profile
{
    public ProductionAutoMapperProfile()
    {
        CreateMap<MaterialRequisition, MaterialRequisitionOutputDto>()
            .ForMember(d => d.RequisitionStatusDescription, opt => opt.MapFrom(s => s.RequisitionStatus.Description));
        CreateMap<MaterialRequisitionLine, MaterialRequisitionLineOutputDto>();
    }
}
