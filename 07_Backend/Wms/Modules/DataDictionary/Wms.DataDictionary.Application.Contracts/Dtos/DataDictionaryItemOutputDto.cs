namespace Wms.DataDictionary.Application.Contracts.Dtos;

public class DictionaryItemOutputDto
{
    public Guid Id { get; set; }
    public Guid DictionaryId { get; set; }
    public string DictionaryCode { get; set; } = string.Empty;
    public string DictionaryName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? ItemValue { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}
