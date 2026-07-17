using System;
using System.Collections.Generic;

namespace Wms.Workflow.Application.Contracts.Dtos;

/// <summary>ApprovalFlow Create DTO</summary>
public class ApprovalFlowCreateDto
{
    public string FlowName { get; set; }
    public int FlowTypeValue { get; set; }
    public string? Description { get; set; }
    public List<ApprovalNodeDto> Nodes { get; set; } = new();
}

/// <summary>ApprovalFlow Update DTO</summary>
public class ApprovalFlowUpdateDto
{
    public string? FlowName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<ApprovalNodeDto> Nodes { get; set; } = new();
}

/// <summary>ApprovalFlow Output DTO</summary>
public class ApprovalFlowOutputDto
{
    public Guid Id { get; set; }
    public string FlowName { get; set; }
    public int FlowTypeValue { get; set; }
    public string FlowTypeDescription { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<ApprovalNodeOutputDto> Nodes { get; set; } = new();
    public DateTime CreationTime { get; set; }
}

/// <summary>ApprovalFlow Query DTO</summary>
public class ApprovalFlowQueryDto : PagedAndSortedResultRequestDto
{
    public int? FlowTypeValue { get; set; }
    public bool? IsActive { get; set; }
    public string? FlowName { get; set; }
}

/// <summary>Approval Node DTO (input)</summary>
public class ApprovalNodeDto
{
    public string NodeName { get; set; }
    public int NodeTypeValue { get; set; }
    public string? ApproverRole { get; set; }
    public Guid? ApproverUserId { get; set; }
    public string? ConditionExpression { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }
}

/// <summary>Approval Node Output DTO</summary>
public class ApprovalNodeOutputDto
{
    public Guid Id { get; set; }
    public string NodeName { get; set; }
    public int NodeTypeValue { get; set; }
    public string NodeTypeDescription { get; set; }
    public string? ApproverRole { get; set; }
    public Guid? ApproverUserId { get; set; }
    public string? ConditionExpression { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }
}
