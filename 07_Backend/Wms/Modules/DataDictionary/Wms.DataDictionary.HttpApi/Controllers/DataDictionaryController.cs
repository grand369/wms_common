using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Wms.DataDictionary.Application.Contracts.Dtos;
using Wms.DataDictionary.Application.Contracts.Services;

namespace Wms.DataDictionary.HttpApi.Controllers;

[Route("api/v1/data-dictionary/dictionaries")]
[ApiController]
public class DataDictionaryController : AbpControllerBase
{
    private readonly IDataDictionaryAppService _service;

    public DataDictionaryController(IDataDictionaryAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<PagedResultDto<DictionaryOutputDto>> GetListAsync([FromQuery] DictionaryQueryDto query)
    {
        return await _service.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public async Task<DictionaryOutputDto> GetAsync(Guid id)
    {
        return await _service.GetAsync(id);
    }

    [HttpPost]
    public async Task<DictionaryOutputDto> CreateAsync([FromBody] DictionaryCreateDto input)
    {
        return await _service.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<DictionaryOutputDto> UpdateAsync(Guid id, [FromBody] DictionaryUpdateDto input)
    {
        return await _service.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
    }
}
