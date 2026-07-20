namespace Wms.DataDictionary.Application.Contracts.Dtos;

public class DictionaryOutputDto
{
    public Guid Id { get; set; }
    public string DictionaryCode { get; set; } = string.Empty;
    public string DictionaryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}
