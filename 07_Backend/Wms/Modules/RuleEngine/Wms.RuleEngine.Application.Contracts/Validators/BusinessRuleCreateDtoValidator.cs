using FluentValidation;
using Wms.RuleEngine.Application.Contracts.Dtos;

namespace Wms.RuleEngine.Application.Contracts.Validators;

/// <summary>
/// BusinessRuleCreateDtoValidator — validates BusinessRule creation DTO.
/// </summary>
public class BusinessRuleCreateDtoValidator : AbstractValidator<BusinessRuleCreateDto>
{
    public BusinessRuleCreateDtoValidator()
    {
        RuleFor(x => x.RuleName)
            .NotEmpty()
            .WithMessage("RuleName is required.")
            .MaximumLength(100)
            .WithMessage("RuleName must not exceed 100 characters.");

        RuleFor(x => x.RuleTypeValue)
            .InclusiveBetween(0, 3)
            .WithMessage("RuleTypeValue must be between 0 (QualityInspection) and 3 (AlertThreshold).");

        RuleFor(x => x.RuleCondition)
            .NotEmpty()
            .WithMessage("RuleCondition is required.");

        RuleFor(x => x.RuleAction)
            .NotEmpty()
            .WithMessage("RuleAction is required.");
    }
}
