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
/// REP-21: ApprovalInstanceRepository — EF Core implementation for ApprovalInstance aggregate.
/// </summary>
public class ApprovalInstanceRepository : EfCoreRepository<WmsWorkflowDbContext, ApprovalInstance, Guid>, IApprovalInstanceRepository
{
    public ApprovalInstanceRepository(IDbContextProvider<WmsWorkflowDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<ApprovalInstance?> GetByBusinessOrderAsync(string businessOrderType, Guid businessOrderId)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(i => i.BusinessOrderType == businessOrderType
                                   && i.BusinessOrderId == businessOrderId);
    }

    public async Task<List<ApprovalInstance>> GetPendingByApproverAsync(Guid approverUserId)
    {
        // Pending or InProgress instances are considered "pending" for an approver
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(i => (i.InstanceStatus == ApprovalInstanceStatus.Pending
                      || i.InstanceStatus == ApprovalInstanceStatus.InProgress
                      || i.InstanceStatus == ApprovalInstanceStatus.Resubmitted)
                      && i.SubmitUserId == approverUserId)
            .ToListAsync();
    }

    public async Task<List<ApprovalInstance>> GetByStatusAsync(ApprovalInstanceStatus status)
    {
        return await (await GetDbSetAsync())
            .Where(i => i.InstanceStatus == status)
            .ToListAsync();
    }
}
