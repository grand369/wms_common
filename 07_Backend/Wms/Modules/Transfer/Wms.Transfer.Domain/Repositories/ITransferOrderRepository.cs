using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.Transfer.Domain.Aggregates;
using Wms.Transfer.Domain.Enums;

namespace Wms.Transfer.Domain.Repositories;

/// <summary>
/// REP-11: ITransferOrderRepository — persistence interface for TransferOrder aggregate.
/// </summary>
public interface ITransferOrderRepository : IBasicRepository<TransferOrder, Guid>
{
    /// <summary>Find by unique order number</summary>
    Task<TransferOrder?> FindByNoAsync(string transferOrderNo);

    /// <summary>Get orders by status</summary>
    Task<List<TransferOrder>> GetByStatusAsync(TransferStatus status);

    /// <summary>Get orders by source warehouse</summary>
    Task<List<TransferOrder>> GetBySourceWarehouseAsync(Guid warehouseId);

    /// <summary>Get orders by target warehouse</summary>
    Task<List<TransferOrder>> GetByTargetWarehouseAsync(Guid warehouseId);

    /// <summary>Get with lines (eager load)</summary>
    Task<TransferOrder> GetWithLinesAsync(Guid id);
}
