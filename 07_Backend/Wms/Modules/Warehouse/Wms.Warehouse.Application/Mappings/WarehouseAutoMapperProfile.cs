using AutoMapper;
using Wms.Warehouse.Domain.Aggregates;
using WarehouseAgg = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Mappings;

/// <summary>
/// Warehouse AutoMapper Profile — configures Domain → DTO mappings.
/// Note: Complex mappings (with enum Description resolution) are done manually in AppService.
/// This profile handles basic property mapping for simpler cases.
/// (Phase 8 Coding Conventions)
/// </summary>
public class WarehouseAutoMapperProfile : Profile
{
    public WarehouseAutoMapperProfile()
    {
        CreateMap<WarehouseAgg, WarehouseOutputDto>()
            .ForSourceMember(w => w.WarehouseType, opt => opt.DoNotValidate())
            .ForSourceMember(w => w.StorageConditionType, opt => opt.DoNotValidate());

        CreateMap<WarehouseArea, WarehouseAreaOutputDto>()
            .ForSourceMember(a => a.AreaFunction, opt => opt.DoNotValidate())
            .ForSourceMember(a => a.StorageEnvironment, opt => opt.DoNotValidate());

        CreateMap<Location, LocationOutputDto>()
            .ForSourceMember(l => l.LocationType, opt => opt.DoNotValidate())
            .ForSourceMember(l => l.StorageCondition, opt => opt.DoNotValidate());
    }
}
