using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Wms.BarcodeLabel.Domain.Repositories;

namespace Wms.BarcodeLabel.EntityFrameworkCore.Repositories;

public class PrintTaskRepository : EfCoreRepository<WmsBarcodeLabelDbContext, PrintTask, Guid>, IPrintTaskRepository
{
    public PrintTaskRepository(IDbContextProvider<WmsBarcodeLabelDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<List<PrintTask>> GetByStatusAsync(PrintTaskStatus status)
        => await (await GetDbSetAsync()).Where(t => t.PrintStatus == status).OrderByDescending(t => t.CreationTime).ToListAsync();

    public async Task<List<PrintTask>> GetByPrinterAsync(string printerId)
        => await (await GetDbSetAsync()).Where(t => t.PrinterId == printerId).OrderByDescending(t => t.CreationTime).ToListAsync();

    public async Task<List<PrintTask>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
        => await (await GetDbSetAsync()).Where(t => t.SourceOrderType == sourceOrderType && t.SourceOrderId == sourceOrderId)
            .OrderByDescending(t => t.CreationTime).ToListAsync();
}
