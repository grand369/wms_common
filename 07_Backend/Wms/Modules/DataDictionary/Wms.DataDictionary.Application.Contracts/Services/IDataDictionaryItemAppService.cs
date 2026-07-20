using Volo.Abp.Application.Services;
using Wms.DataDictionary.Application.Contracts.Dtos;

namespace Wms.DataDictionary.Application.Contracts.Services;

public interface IDataDictionaryItemAppService : IApplicationService
{
    Task<List<DictionaryItemOutputDto>> GetListAsync(Guid dictionaryId);
    Task<List<DictionaryItemOutputDto>> GetListByCodeAsync(string dictionaryCode);
    Task<DictionaryItemOutputDto> GetAsync(Guid id);
    Task<DictionaryItemOutputDto> CreateAsync(DictionaryItemCreateDto input);
    Task<DictionaryItemOutputDto> UpdateAsync(Guid id, DictionaryItemUpdateDto input);
    Task DeleteAsync(Guid id);
}
