using AutoMapper;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Wms.DataDictionary.Application.Contracts.Dtos;
using Wms.DataDictionary.Application.Contracts.Services;
using Wms.DataDictionary.Application.Contracts.Permissions;
using Wms.DataDictionary.Domain.Entities;
using Volo.Abp.Authorization;
using Volo.Abp.Linq;

namespace Wms.DataDictionary.Application.Services;

[Authorize(WmsDataDictionaryPermissions.Dictionaries.Default)]
public class DataDictionaryAppService : ApplicationService, IDataDictionaryAppService
{
    private readonly IRepository<Dictionary, Guid> _repository;
    private readonly IMapper _mapper;

    public DataDictionaryAppService(IRepository<Dictionary, Guid> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<DictionaryOutputDto>> GetListAsync(DictionaryQueryDto query)
    {
        var queryable = await _repository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.DictionaryCode), d => d.DictionaryCode.Contains(query.DictionaryCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.DictionaryName), d => d.DictionaryName.Contains(query.DictionaryName!))
            .WhereIf(query.IsActive.HasValue, d => d.IsActive == query.IsActive.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(d => d.CreationTime).Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<DictionaryOutputDto>(totalCount, _mapper.Map<List<Dictionary>, List<DictionaryOutputDto>>(items));
    }

    [AllowAnonymous]
    public async Task<DictionaryOutputDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return _mapper.Map<Dictionary, DictionaryOutputDto>(entity);
    }

    [Authorize(WmsDataDictionaryPermissions.Dictionaries.Create)]
    public async Task<DictionaryOutputDto> CreateAsync(DictionaryCreateDto input)
    {
        var entity = _mapper.Map<DictionaryCreateDto, Dictionary>(input);
        await _repository.InsertAsync(entity);
        return _mapper.Map<Dictionary, DictionaryOutputDto>(entity);
    }

    [Authorize(WmsDataDictionaryPermissions.Dictionaries.Update)]
    public async Task<DictionaryOutputDto> UpdateAsync(Guid id, DictionaryUpdateDto input)
    {
        var entity = await _repository.GetAsync(id);
        _mapper.Map(input, entity);
        await _repository.UpdateAsync(entity);
        return _mapper.Map<Dictionary, DictionaryOutputDto>(entity);
    }

    [Authorize(WmsDataDictionaryPermissions.Dictionaries.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
