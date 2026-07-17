using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Repositories;

/// <summary>
/// IPrintTaskRepository (REP-19) — custom query methods for PrintTask aggregate.
/// </summary>
public interface IPrintTaskRepository : IRepository<PrintTask, Guid>
{
    /// <summary>Get print tasks by status.</summary>
    Task<List<PrintTask>> GetByStatusAsync(PrintTaskStatus status);

    /// <summary>Get print tasks by printer ID.</summary>
    Task<List<PrintTask>> GetByPrinterAsync(string printerId);

    /// <summary>Get print tasks by source order.</summary>
    Task<List<PrintTask>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId);
}
