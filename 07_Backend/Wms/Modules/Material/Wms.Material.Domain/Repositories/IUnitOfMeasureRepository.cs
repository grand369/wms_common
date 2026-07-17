using Volo.Abp.Domain.Repositories;
using Wms.Material.Domain.Entities;

namespace Wms.Material.Domain.Repositories;

/// <summary>
/// Unit of Measure Repository Interface — extends ABP IRepository with custom query methods.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public interface IUnitOfMeasureRepository : IRepository<UnitOfMeasure, Guid>
{
    Task<UnitOfMeasure?> FindByCodeAsync(string unitCode);
    Task<List<UnitOfMeasure>> GetActiveListAsync();
    Task<bool> CodeExistsAsync(string unitCode);
}
