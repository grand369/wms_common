using FluentValidation;
using Wms.Workflow.Application.Contracts.Dtos;

namespace Wms.Workflow.Application.Contracts.Validators;

/// <summary>Validator for StartApprovalDto</summary>
public class StartApprovalDtoValidator : AbstractValidator<StartApprovalDto>
{
    public StartApprovalDtoValidator()
    {
        RuleFor(x => x.FlowId)
            .NotEmpty().WithMessage("Flow ID is required.");

        RuleFor(x => x.BusinessOrderId)
            .NotEmpty().WithMessage("Business order ID is required.");

        RuleFor(x => x.BusinessOrderType)
            .NotEmpty().WithMessage("Business order type is required.")
            .MaximumLength(50);
    }
}
