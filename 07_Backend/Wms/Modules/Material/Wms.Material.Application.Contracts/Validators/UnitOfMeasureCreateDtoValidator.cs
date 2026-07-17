using FluentValidation;
using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Validators;

/// <summary>
/// Unit of Measure Create DTO Validator.
/// </summary>
public class UnitOfMeasureCreateDtoValidator : AbstractValidator<UnitOfMeasureCreateDto>
{
    public UnitOfMeasureCreateDtoValidator()
    {
        RuleFor(x => x.UnitCode).NotEmpty().WithMessage("单位编码不能为空").MaximumLength(50);
        RuleFor(x => x.UnitName).NotEmpty().WithMessage("单位名称不能为空").MaximumLength(100);
        RuleFor(x => x.UnitSymbol).NotEmpty().WithMessage("单位符号不能为空").MaximumLength(20);
        RuleFor(x => x.UnitType).InclusiveBetween(0, 6).WithMessage("单位类型值必须在0-6之间");
    }
}
