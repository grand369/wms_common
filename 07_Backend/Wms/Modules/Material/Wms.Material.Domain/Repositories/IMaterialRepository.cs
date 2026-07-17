using Volo.Abp.Domain.Repositories;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;

namespace Wms.Material.Domain.Repositories;

/// <summary>
/// Material Repository Interface — extends ABP IRepository with custom query methods.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public interface IMaterialRepository : IRepository<MaterialAgg, Guid>
{
    Task<MaterialAgg?> FindByCodeAsync(string materialCode);
    Task<List<MaterialAgg>> GetListByClassificationIdAsync(Guid classificationId);
    Task<List<MaterialAgg>> GetListByTypeAsync(int materialType);
    Task<List<MaterialAgg>> GetActiveListAsync();
    Task<bool> CodeExistsAsync(string materialCode);
}
