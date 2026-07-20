namespace Wms.DataDictionary.Application.Contracts.Dtos;

public class DictionaryQueryDto
{
    public string? DictionaryCode { get; set; }
    public string? DictionaryName { get; set; }
    public bool? IsActive { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 10;
    public string? Sorting { get; set; }
}
