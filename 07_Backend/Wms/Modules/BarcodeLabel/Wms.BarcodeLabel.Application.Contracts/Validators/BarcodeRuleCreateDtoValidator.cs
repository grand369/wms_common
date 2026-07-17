using FluentValidation;
using Wms.BarcodeLabel.Application.Contracts.Dtos;

namespace Wms.BarcodeLabel.Application.Contracts.Validators;

public class BarcodeRuleCreateDtoValidator : AbstractValidator<BarcodeRuleCreateDto>
{
    public BarcodeRuleCreateDtoValidator()
    {
        RuleFor(x => x.RuleName).NotEmpty().MaximumLength(100)
            .WithMessage("RuleName is required and must not exceed 100 characters.");

        RuleFor(x => x.BarcodeTypeValue).InclusiveBetween(0, 4)
            .WithMessage("BarcodeTypeValue must be between 0 (Material) and 4 (Serial).");

        RuleFor(x => x.BarcodeFormatValue).InclusiveBetween(0, 3)
            .WithMessage("BarcodeFormatValue must be between 0 (QR) and 3 (EAN13).");

        RuleFor(x => x.CodePattern).NotEmpty()
            .WithMessage("CodePattern is required.");
    }
}
