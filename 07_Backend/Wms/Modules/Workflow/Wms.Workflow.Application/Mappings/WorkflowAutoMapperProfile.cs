using AutoMapper;
using Wms.Workflow.Application.Contracts.Dtos;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;

namespace Wms.Workflow.Application.Mappings;

/// <summary>
/// AutoMapper profile for Workflow module.
/// Maps ApprovalFlow, ApprovalInstance, ApprovalNode, ApprovalActionLog to/from DTOs.
/// </summary>
public class WorkflowAutoMapperProfile : Profile
{
    public WorkflowAutoMapperProfile()
    {
        // ── ApprovalFlow ────────────────────────────────────
        CreateMap<ApprovalFlow, ApprovalFlowOutputDto>()
            .ForMember(d => d.FlowTypeValue, opt => opt.MapFrom(s => s.FlowType.Value))
            .ForMember(d => d.FlowTypeDescription, opt => opt.MapFrom(s => s.FlowType.Description))
            .ForMember(d => d.Nodes, opt => opt.MapFrom(s => s.Nodes))
            .ForMember(d => d.CreationTime, opt => opt.MapFrom(s => s.CreationTime));

        CreateMap<ApprovalNode, ApprovalNodeOutputDto>()
            .ForMember(d => d.NodeTypeValue, opt => opt.MapFrom(s => s.NodeType.Value))
            .ForMember(d => d.NodeTypeDescription, opt => opt.MapFrom(s => s.NodeType.Description));

        // ── ApprovalInstance ────────────────────────────────
        CreateMap<ApprovalInstance, ApprovalInstanceOutputDto>()
            .ForMember(d => d.InstanceStatusValue, opt => opt.MapFrom(s => s.InstanceStatus.Value))
            .ForMember(d => d.InstanceStatusDescription, opt => opt.MapFrom(s => s.InstanceStatus.Description))
            .ForMember(d => d.ActionLogs, opt => opt.MapFrom(s => s.ActionLogs));

        CreateMap<ApprovalActionLog, ApprovalActionLogOutputDto>()
            .ForMember(d => d.ActionTypeValue, opt => opt.MapFrom(s => s.ActionType.Value))
            .ForMember(d => d.ActionTypeDescription, opt => opt.MapFrom(s => s.ActionType.Description));
    }
}
