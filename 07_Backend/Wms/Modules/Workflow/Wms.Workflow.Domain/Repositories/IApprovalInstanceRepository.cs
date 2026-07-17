using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;

namespace Wms.Workflow.Domain.Repositories;

/// <summary>
/// REP-21: IApprovalInstanceRepository — persistence interface for ApprovalInstance aggregate.
/// </summary>
public interface IApprovalInstanceRepository : IBasicRepository<ApprovalInstance, Guid>
{
    /// <summary>Get instance by business order type and id.</summary>
    Task<ApprovalInstance?> GetByBusinessOrderAsync(string businessOrderType, Guid businessOrderId);

    /// <summary>Get pending instances by approver user id.</summary>
    Task<List<ApprovalInstance>> GetPendingByApproverAsync(Guid approverUserId);

    /// <summary>Get instances by status.</summary>
    Task<List<ApprovalInstance>> GetByStatusAsync(ApprovalInstanceStatus status);
}
