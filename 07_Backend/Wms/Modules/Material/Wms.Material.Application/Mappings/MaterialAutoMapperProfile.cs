using AutoMapper;
using Wms.Material.Domain.Aggregates;
using MaterialEntity = Wms.Material.Domain.Aggregates.Material;
using Wms.Material.Domain.Entities;
using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Mappings;

/// <summary>
/// Material AutoMapper Profile — basic Domain → DTO mapping profile.
/// Complex mappings with enum Description resolution are done manually in AppService.
/// </summary>
public class MaterialAutoMapperProfile : Profile
{
    public MaterialAutoMapperProfile()
    {
        CreateMap<MaterialEntity, MaterialOutputDto>();
        CreateMap<MaterialClassification, MaterialClassificationOutputDto>();
        CreateMap<UnitOfMeasure, UnitOfMeasureOutputDto>();
        CreateMap<MaterialSubstituteRelation, MaterialSubstituteRelationDto>();
    }
}
