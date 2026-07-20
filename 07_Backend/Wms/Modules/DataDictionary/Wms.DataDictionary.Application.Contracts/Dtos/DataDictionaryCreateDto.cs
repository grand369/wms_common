using System.ComponentModel.DataAnnotations;

namespace Wms.DataDictionary.Application.Contracts.Dtos;

public class DictionaryCreateDto
{
    [Required] [StringLength(50)] public string DictionaryCode { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string DictionaryName { get; set; } = string.Empty;
    [StringLength(500)] public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
