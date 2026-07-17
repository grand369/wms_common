using FluentValidation;
using Wms.Workflow.Application.Contracts.Dtos;

namespace Wms.Workflow.Application.Contracts.Validators;

/// <summary>Validator for ApprovalFlowCreateDto</summary>
public class ApprovalFlowCreateDtoValidator : AbstractValidator<ApprovalFlowCreateDto>
{
    public ApprovalFlowCreateDtoValidator()
    {
        RuleFor(x => x.FlowName)
            .NotEmpty().WithMessage("Flow name is required.")
            .MaximumLength(100);

        RuleFor(x => x.FlowTypeValue)
            .InclusiveBetween(0, 4).WithMessage("Flow type value must be 0-4.");

        RuleFor(x => x.Nodes)
            .NotEmpty().WithMessage("At least one approval node is required.");

        RuleForEach(x => x.Nodes).ChildRules(node =>
        {
            node.RuleFor(n => n.NodeName).NotEmpty().MaximumLength(100);
            node.RuleFor(n => n.NodeTypeValue).InclusiveBetween(0, 3);
            node.RuleFor(n => n.Order).GreaterThanOrEqualTo(0);
        });
    }
}
