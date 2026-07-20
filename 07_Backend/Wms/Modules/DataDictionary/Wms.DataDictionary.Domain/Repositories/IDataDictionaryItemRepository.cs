using Volo.Abp.Domain.Repositories;
using Wms.DataDictionary.Domain.Entities;

namespace Wms.DataDictionary.Domain.Repositories;

public interface IDataDictionaryItemRepository : IRepository<DataDictionaryItem, Guid>
{
    Task<List<DataDictionaryItem>> GetItemsByDictionaryIdAsync(Guid dictionaryId);
    Task<List<DataDictionaryItem>> GetItemsByDictionaryCodeAsync(string dictionaryCode);
}
