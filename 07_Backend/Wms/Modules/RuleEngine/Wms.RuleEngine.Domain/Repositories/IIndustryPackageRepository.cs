using Volo.Abp.Domain.Repositories;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;

namespace Wms.RuleEngine.Domain.Repositories;

/// <summary>
/// IIndustryPackageRepository (REP-23) — custom query methods for IndustryPackage aggregate.
/// </summary>
public interface IIndustryPackageRepository : IRepository<IndustryPackage, Guid>
{
    /// <summary>Find industry package by package name.</summary>
    Task<IndustryPackage?> FindByPackageNameAsync(string packageName);

    /// <summary>Get industry packages by industry type.</summary>
    Task<List<IndustryPackage>> GetByIndustryTypeAsync(IndustryType industryType);
}
