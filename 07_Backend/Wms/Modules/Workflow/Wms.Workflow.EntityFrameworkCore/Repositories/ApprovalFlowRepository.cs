using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;
using Wms.Workflow.Domain.Repositories;

namespace Wms.Workflow.EntityFrameworkCore.Repositories;

/// <summary>
/// REP-20: ApprovalFlowRepository — EF Core implementation for ApprovalFlow aggregate.
/// </summary>
public class ApprovalFlowRepository : EfCoreRepository<WmsWorkflowDbContext, ApprovalFlow, Guid>, IApprovalFlowRepository
{
    public ApprovalFlowRepository(IDbContextProvider<WmsWorkflowDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<ApprovalFlow?> FindByFlowNameAsync(string flowName)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(f => f.FlowName == flowName);
    }

    public async Task<List<ApprovalFlow>> GetByFlowTypeAsync(ApprovalFlowType flowType)
    {
        return await (await GetDbSetAsync())
            .Where(f => f.FlowType == flowType)
            .ToListAsync();
    }

    public async Task<List<ApprovalFlow>> GetActiveFlowsAsync()
    {
        return await (await GetDbSetAsync())
            .Where(f => f.IsActive)
            .ToListAsync();
    }
}
