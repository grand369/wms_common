using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Transfer.Domain.Aggregates;
using Wms.Transfer.Domain.Enums;
using Wms.Transfer.Domain.Repositories;

namespace Wms.Transfer.EntityFrameworkCore.Repositories;

/// <summary>
/// REP-11: TransferOrderRepository — EF Core implementation
/// </summary>
public class TransferOrderRepository : EfCoreRepository<WmsTransferDbContext, TransferOrder, Guid>, ITransferOrderRepository
{
    public TransferOrderRepository(IDbContextProvider<WmsTransferDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<TransferOrder?> FindByNoAsync(string transferOrderNo)
    {
        return await (await GetDbSetAsync()).FirstOrDefaultAsync(o => o.TransferOrderNo == transferOrderNo);
    }

    public async Task<List<TransferOrder>> GetByStatusAsync(TransferStatus status)
    {
        return await (await GetDbSetAsync()).Where(o => o.TransferStatus == status).ToListAsync();
    }

    public async Task<List<TransferOrder>> GetBySourceWarehouseAsync(Guid warehouseId)
    {
        return await (await GetDbSetAsync()).Where(o => o.SourceWarehouseId == warehouseId).ToListAsync();
    }

    public async Task<List<TransferOrder>> GetByTargetWarehouseAsync(Guid warehouseId)
    {
        return await (await GetDbSetAsync()).Where(o => o.TargetWarehouseId == warehouseId).ToListAsync();
    }

    public async Task<TransferOrder> GetWithLinesAsync(Guid id)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id)
            ?? throw new EntityNotFoundException(typeof(TransferOrder), id);
    }
}
