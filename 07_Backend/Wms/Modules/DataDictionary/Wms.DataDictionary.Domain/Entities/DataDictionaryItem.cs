using Volo.Abp.Domain.Entities;

namespace Wms.DataDictionary.Domain.Entities;

public class DataDictionaryItem : AggregateRoot<Guid>
{
    public Guid DictionaryId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? ItemValue { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }

    public virtual Dictionary? Dict { get; set; }
}
