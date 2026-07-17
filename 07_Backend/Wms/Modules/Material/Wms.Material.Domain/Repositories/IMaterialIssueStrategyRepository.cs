using Volo.Abp.Domain.Repositories;
using Wms.Material.Domain.Aggregates;

namespace Wms.Material.Domain.Repositories;

public interface IMaterialIssueStrategyRepository : IRepository<MaterialIssueStrategy, Guid>
{
    Task<bool> CodeExistsAsync(string code);
    Task<MaterialIssueStrategy?> FindByCodeAsync(string code);
}