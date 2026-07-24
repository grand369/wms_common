using Volo.Abp.Domain.Repositories;
using SupplierAgg = Wms.Supplier.Domain.Aggregates.Supplier;

namespace Wms.Supplier.Domain.Repositories;

/// <summary>
/// Supplier Repository Interface — defines data access operations for Supplier aggregate.
/// </summary>
public interface ISupplierRepository : IRepository<SupplierAgg, Guid>
{
    Task<SupplierAgg?> GetByCodeAsync(string supplierCode, bool includeDetails = false);
    
    Task<bool> ExistsByCodeAsync(string supplierCode, Guid? excludeId = null);
    
    Task<List<SupplierAgg>> GetActiveSuppliersAsync();
}
