using System.ComponentModel.DataAnnotations;

namespace Wms.DataDictionary.Application.Contracts.Dtos;

public class DictionaryItemCreateDto
{
    [Required] public Guid DictionaryId { get; set; }
    [Required] [StringLength(50)] public string ItemCode { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string ItemName { get; set; } = string.Empty;
    [StringLength(500)] public string? ItemValue { get; set; }
    [StringLength(500)] public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
