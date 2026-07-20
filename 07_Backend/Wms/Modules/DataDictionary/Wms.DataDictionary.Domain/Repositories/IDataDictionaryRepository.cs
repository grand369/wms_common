using Volo.Abp.Domain.Repositories;
using Wms.DataDictionary.Domain.Entities;

namespace Wms.DataDictionary.Domain.Repositories;

public interface IDataDictionaryRepository : IRepository<Dictionary, Guid>
{
    Task<Dictionary?> FindByCodeAsync(string code);
}
