using FluentValidation;
using Wms.RuleEngine.Application.Contracts.Dtos;

namespace Wms.RuleEngine.Application.Contracts.Validators;

/// <summary>
/// IndustryPackageCreateDtoValidator — validates IndustryPackage creation DTO.
/// </summary>
public class IndustryPackageCreateDtoValidator : AbstractValidator<IndustryPackageCreateDto>
{
    public IndustryPackageCreateDtoValidator()
    {
        RuleFor(x => x.PackageName)
            .NotEmpty()
            .WithMessage("PackageName is required.")
            .MaximumLength(100)
            .WithMessage("PackageName must not exceed 100 characters.");

        RuleFor(x => x.IndustryTypeValue)
            .InclusiveBetween(0, 4)
            .WithMessage("IndustryTypeValue must be between 0 (Automotive) and 4 (General).");

        RuleFor(x => x.PackageContent)
            .NotEmpty()
            .WithMessage("PackageContent is required.");
    }
}
