using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using SupplierEntity = Wms.Supplier.Domain.Aggregates.Supplier;
using Wms.Supplier.Domain.Repositories;

namespace Wms.Supplier.EntityFrameworkCore.Repositories;

/// <summary>
/// Supplier Repository — implements ISupplierRepository using EF Core.
/// </summary>
public class SupplierRepository : EfCoreRepository<WmsSupplierDbContext, SupplierEntity, Guid>, ISupplierRepository
{
    public SupplierRepository(IDbContextProvider<WmsSupplierDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<SupplierEntity?> GetByCodeAsync(string supplierCode, bool includeDetails = false)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Suppliers
            .FirstOrDefaultAsync(s => s.SupplierCode == supplierCode);
    }

    public async Task<bool> ExistsByCodeAsync(string supplierCode, Guid? excludeId = null)
    {
        var dbContext = await GetDbContextAsync();
        var query = dbContext.Suppliers.Where(s => s.SupplierCode == supplierCode);
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<List<SupplierEntity>> GetActiveSuppliersAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.SupplierCode)
            .ToListAsync();
    }
}