using System;
using System.Collections.Generic;

namespace Wms.Workflow.Application.Contracts.Dtos;

/// <summary>ApprovalInstance Output DTO</summary>
public class ApprovalInstanceOutputDto
{
    public Guid Id { get; set; }
    public Guid FlowId { get; set; }
    public string? FlowName { get; set; }
    public int InstanceStatusValue { get; set; }
    public string InstanceStatusDescription { get; set; }
    public Guid BusinessOrderId { get; set; }
    public string BusinessOrderType { get; set; }
    public string? BusinessOrderNo { get; set; }
    public Guid? CurrentNodeId { get; set; }
    public string? CurrentNodeName { get; set; }
    public Guid SubmitUserId { get; set; }
    public string? SubmitUserName { get; set; }
    public DateTime SubmitTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public List<ApprovalActionLogOutputDto> ActionLogs { get; set; } = new();
}

/// <summary>ApprovalInstance Query DTO</summary>
public class ApprovalInstanceQueryDto : PagedAndSortedResultRequestDto
{
    public int? InstanceStatusValue { get; set; }
    public string? BusinessOrderType { get; set; }
    public Guid? SubmitUserId { get; set; }
}

/// <summary>Start Approval DTO</summary>
public class StartApprovalDto
{
    public Guid FlowId { get; set; }
    public Guid BusinessOrderId { get; set; }
    public string BusinessOrderType { get; set; }
    public string? BusinessOrderNo { get; set; }
}

/// <summary>Approval Action DTO</summary>
public class ApprovalActionDto
{
    public Guid InstanceId { get; set; }
    public int ActionTypeValue { get; set; }
    public string? Comment { get; set; }
}

/// <summary>ApprovalActionLog Output DTO</summary>
public class ApprovalActionLogOutputDto
{
    public Guid Id { get; set; }
    public Guid NodeId { get; set; }
    public string? NodeName { get; set; }
    public Guid ActionUserId { get; set; }
    public string? ActionUserName { get; set; }
    public int ActionTypeValue { get; set; }
    public string ActionTypeDescription { get; set; }
    public string? Comment { get; set; }
    public DateTime ActionTime { get; set; }
}
