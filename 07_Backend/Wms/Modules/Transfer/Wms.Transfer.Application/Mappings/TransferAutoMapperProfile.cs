using AutoMapper;
using Wms.Transfer.Application.Contracts.Dtos;
using Wms.Transfer.Domain.Aggregates;

namespace Wms.Transfer.Application.Mappings;

/// <summary>
/// AutoMapper profile for Transfer module
/// </summary>
public class TransferAutoMapperProfile : Profile
{
    public TransferAutoMapperProfile()
    {
        CreateMap<TransferOrder, TransferOrderOutputDto>()
            .ForMember(d => d.TransferTypeDescription, opt => opt.MapFrom(s => s.TransferType.Description))
            .ForMember(d => d.TransferStatusDescription, opt => opt.MapFrom(s => s.TransferStatus.Description))
            .ForMember(d => d.ApprovalStatusDescription, opt => opt.MapFrom(s => s.ApprovalStatus.Description));
        CreateMap<TransferLine, TransferLineOutputDto>();
    }
}
