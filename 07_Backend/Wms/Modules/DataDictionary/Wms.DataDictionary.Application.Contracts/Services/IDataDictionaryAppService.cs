using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Wms.DataDictionary.Application.Contracts.Dtos;

namespace Wms.DataDictionary.Application.Contracts.Services;

public interface IDataDictionaryAppService : IApplicationService
{
    Task<PagedResultDto<DictionaryOutputDto>> GetListAsync(DictionaryQueryDto query);
    Task<DictionaryOutputDto> GetAsync(Guid id);
    Task<DictionaryOutputDto> CreateAsync(DictionaryCreateDto input);
    Task<DictionaryOutputDto> UpdateAsync(Guid id, DictionaryUpdateDto input);
    Task DeleteAsync(Guid id);
}
