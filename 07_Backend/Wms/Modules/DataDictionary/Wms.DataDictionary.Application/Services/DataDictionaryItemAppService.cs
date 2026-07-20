using AutoMapper;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Wms.DataDictionary.Application.Contracts.Dtos;
using Wms.DataDictionary.Application.Contracts.Services;
using Wms.DataDictionary.Application.Contracts.Permissions;
using Wms.DataDictionary.Domain.Entities;
using Volo.Abp.Authorization;

namespace Wms.DataDictionary.Application.Services;

[Authorize(WmsDataDictionaryPermissions.Items.Default)]
public class DataDictionaryItemAppService : ApplicationService, IDataDictionaryItemAppService
{
    private readonly IRepository<DataDictionaryItem, Guid> _itemRepository;
    private readonly IRepository<Dictionary, Guid> _dictionaryRepository;
    private readonly IMapper _mapper;

    public DataDictionaryItemAppService(
        IRepository<DataDictionaryItem, Guid> itemRepository,
        IRepository<Dictionary, Guid> dictionaryRepository,
        IMapper mapper)
    {
        _itemRepository = itemRepository;
        _dictionaryRepository = dictionaryRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<List<DictionaryItemOutputDto>> GetListAsync(Guid dictionaryId)
    {
        var list = await _itemRepository.GetListAsync(x => x.DictionaryId == dictionaryId);
        var dictionary = await _dictionaryRepository.FindAsync(dictionaryId);
        var result = _mapper.Map<List<DataDictionaryItem>, List<DictionaryItemOutputDto>>(list);
        if (dictionary != null)
        {
            foreach (var item in result)
            {
                item.DictionaryCode = dictionary.DictionaryCode;
                item.DictionaryName = dictionary.DictionaryName;
            }
        }
        return result;
    }

    [AllowAnonymous]
    public async Task<List<DictionaryItemOutputDto>> GetListByCodeAsync(string dictionaryCode)
    {
        var dictionary = await _dictionaryRepository.FirstOrDefaultAsync(x => x.DictionaryCode == dictionaryCode);
        if (dictionary == null) return new List<DictionaryItemOutputDto>();
        return await GetListAsync(dictionary.Id);
    }

    [AllowAnonymous]
    public async Task<DictionaryItemOutputDto> GetAsync(Guid id)
    {
        var entity = await _itemRepository.GetAsync(id);
        var dictionary = await _dictionaryRepository.FindAsync(entity.DictionaryId);
        var result = _mapper.Map<DataDictionaryItem, DictionaryItemOutputDto>(entity);
        if (dictionary != null)
        {
            result.DictionaryCode = dictionary.DictionaryCode;
            result.DictionaryName = dictionary.DictionaryName;
        }
        return result;
    }

    [Authorize(WmsDataDictionaryPermissions.Items.Create)]
    public async Task<DictionaryItemOutputDto> CreateAsync(DictionaryItemCreateDto input)
    {
        var entity = _mapper.Map<DictionaryItemCreateDto, DataDictionaryItem>(input);
        var item = await _itemRepository.InsertAsync(entity);
        return _mapper.Map< DataDictionaryItem,DictionaryItemOutputDto>(item);
    }

    [Authorize(WmsDataDictionaryPermissions.Items.Update)]
    public async Task<DictionaryItemOutputDto> UpdateAsync(Guid id, DictionaryItemUpdateDto input)
    {
        var entity = await _itemRepository.GetAsync(id);
        _mapper.Map(input, entity);
        await _itemRepository.UpdateAsync(entity);
        return await GetAsync(entity.Id);
    }

    [Authorize(WmsDataDictionaryPermissions.Items.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _itemRepository.DeleteAsync(id);
    }
}
