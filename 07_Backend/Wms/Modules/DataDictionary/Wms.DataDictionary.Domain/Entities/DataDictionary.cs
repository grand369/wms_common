using Volo.Abp.Domain.Entities;

namespace Wms.DataDictionary.Domain.Entities;

public class Dictionary : AggregateRoot<Guid>
{
    public string DictionaryCode { get; set; } = string.Empty;
    public string DictionaryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
}
