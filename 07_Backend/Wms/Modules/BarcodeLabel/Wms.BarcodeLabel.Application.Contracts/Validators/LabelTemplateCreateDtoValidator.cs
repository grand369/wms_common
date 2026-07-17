using FluentValidation;
using Wms.BarcodeLabel.Application.Contracts.Dtos;

namespace Wms.BarcodeLabel.Application.Contracts.Validators;

public class LabelTemplateCreateDtoValidator : AbstractValidator<LabelTemplateCreateDto>
{
    public LabelTemplateCreateDtoValidator()
    {
        RuleFor(x => x.TemplateName).NotEmpty().MaximumLength(100)
            .WithMessage("TemplateName is required and must not exceed 100 characters.");

        RuleFor(x => x.TemplateTypeValue).InclusiveBetween(0, 3)
            .WithMessage("TemplateTypeValue must be between 0 (Inbound) and 3 (Customer).");

        RuleFor(x => x.TemplateContent).NotEmpty()
            .WithMessage("TemplateContent is required.");
    }
}
