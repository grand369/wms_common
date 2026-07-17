using Volo.Abp.Domain.Repositories;
using Wms.Material.Domain.Aggregates;

namespace Wms.Material.Domain.Repositories;

/// <summary>
/// Material Classification Repository Interface — extends ABP IRepository with custom query methods.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public interface IMaterialClassificationRepository : IRepository<MaterialClassification, Guid>
{
    Task<MaterialClassification?> FindByCodeAsync(string classificationCode);
    Task<List<MaterialClassification>> GetTreeAsync();
    Task<List<MaterialClassification>> GetListByParentIdAsync(Guid? parentId);
    Task<bool> CodeExistsAsync(string classificationCode);
}
