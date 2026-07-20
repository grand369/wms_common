using AutoMapper;
using Wms.DataDictionary.Application.Contracts.Dtos;
using Wms.DataDictionary.Domain.Entities;

namespace Wms.DataDictionary.Application.Mappings;

public class DataDictionaryAutoMapperProfile : Profile
{
    public DataDictionaryAutoMapperProfile()
    {
        CreateMap<Dictionary, DictionaryOutputDto>();
        CreateMap<DictionaryCreateDto, Dictionary>();
        CreateMap<DictionaryUpdateDto, Dictionary>();

        CreateMap<DataDictionaryItem, DictionaryItemOutputDto>();
        CreateMap<DictionaryItemCreateDto, DataDictionaryItem>();
        CreateMap<DictionaryItemUpdateDto, DataDictionaryItem>();
    }
}
