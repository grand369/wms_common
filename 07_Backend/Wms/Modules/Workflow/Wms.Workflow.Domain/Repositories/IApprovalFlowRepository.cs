using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;

namespace Wms.Workflow.Domain.Repositories;

/// <summary>
/// REP-20: IApprovalFlowRepository — persistence interface for ApprovalFlow aggregate.
/// </summary>
public interface IApprovalFlowRepository : IBasicRepository<ApprovalFlow, Guid>
{
    /// <summary>Find by flow name.</summary>
    Task<ApprovalFlow?> FindByFlowNameAsync(string flowName);

    /// <summary>Get flows by flow type.</summary>
    Task<List<ApprovalFlow>> GetByFlowTypeAsync(ApprovalFlowType flowType);

    /// <summary>Get all active flows.</summary>
    Task<List<ApprovalFlow>> GetActiveFlowsAsync();
}
