using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.DataDictionary.Application.Contracts.Dtos;
using Wms.DataDictionary.Application.Contracts.Services;

namespace Wms.DataDictionary.HttpApi.Controllers;

[Route("api/v1/data-dictionary/items")]
[ApiController]
public class DataDictionaryItemController : AbpControllerBase
{
    private readonly IDataDictionaryItemAppService _service;

    public DataDictionaryItemController(IDataDictionaryItemAppService service)
    {
        _service = service;
    }

    [HttpGet("by-dictionary/{dictionaryId}")]
    public async Task<List<DictionaryItemOutputDto>> GetListByDictionaryIdAsync(Guid dictionaryId)
    {
        return await _service.GetListAsync(dictionaryId);
    }

    [HttpGet("by-code/{dictionaryCode}")]
    public async Task<List<DictionaryItemOutputDto>> GetListByDictionaryCodeAsync(string dictionaryCode)
    {
        return await _service.GetListByCodeAsync(dictionaryCode);
    }

    [HttpGet("{id}")]
    public async Task<DictionaryItemOutputDto> GetAsync(Guid id)
    {
        return await _service.GetAsync(id);
    }

    [HttpPost]
    public async Task<DictionaryItemOutputDto> CreateAsync([FromBody] DictionaryItemCreateDto input)
    {
        return await _service.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<DictionaryItemOutputDto> UpdateAsync(Guid id, [FromBody] DictionaryItemUpdateDto input)
    {
        return await _service.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
    }
}
