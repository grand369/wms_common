using AutoMapper;
using Wms.Supplier.Application.Contracts.Dtos;
using SupplierAgg = Wms.Supplier.Domain.Aggregates.Supplier;

namespace Wms.Supplier.Application.Mappings;

/// <summary>
/// Supplier AutoMapper Profile — defines mapping rules between entities and DTOs.
/// </summary>
public class SupplierAutoMapperProfile : Profile
{
    public SupplierAutoMapperProfile()
    {
        CreateMap<SupplierAgg, SupplierOutputDto>()
            .ForMember(dest => dest.SupplierTypeDescription, opt => opt.MapFrom(src => GetSupplierTypeDescription(src.SupplierType)));

        CreateMap<SupplierCreateDto, SupplierAgg>(MemberList.Source);

        CreateMap<SupplierUpdateDto, SupplierAgg>(MemberList.Source);
    }

    private string? GetSupplierTypeDescription(int supplierType)
    {
        return supplierType switch
        {
            1 => "普通供应商",
            2 => "战略供应商",
            3 => "委外加工商",
            _ => null
        };
    }
}
