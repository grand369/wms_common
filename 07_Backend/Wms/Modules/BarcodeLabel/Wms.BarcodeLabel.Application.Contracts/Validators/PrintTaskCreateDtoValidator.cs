using FluentValidation;
using Wms.BarcodeLabel.Application.Contracts.Dtos;

namespace Wms.BarcodeLabel.Application.Contracts.Validators;

public class PrintTaskCreateDtoValidator : AbstractValidator<PrintTaskCreateDto>
{
    public PrintTaskCreateDtoValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty()
            .WithMessage("TemplateId is required.");

        RuleFor(x => x.SourceOrderType).NotEmpty()
            .WithMessage("SourceOrderType is required.");

        RuleFor(x => x.PrintContent).NotEmpty()
            .WithMessage("PrintContent is required.");

        RuleFor(x => x.PrintQuantity).GreaterThan(0)
            .WithMessage("PrintQuantity must be greater than 0.");
    }
}
