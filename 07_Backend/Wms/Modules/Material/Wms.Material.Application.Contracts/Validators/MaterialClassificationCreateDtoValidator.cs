using FluentValidation;
using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Validators;

/// <summary>
/// Material Classification Create DTO Validator.
/// </summary>
public class MaterialClassificationCreateDtoValidator : AbstractValidator<MaterialClassificationCreateDto>
{
    public MaterialClassificationCreateDtoValidator()
    {
        RuleFor(x => x.ClassificationCode).NotEmpty().WithMessage("分类编码不能为空").MaximumLength(50);
        RuleFor(x => x.ClassificationName).NotEmpty().WithMessage("分类名称不能为空").MaximumLength(200);
        RuleFor(x => x.ClassificationLevel).InclusiveBetween(1, 10).WithMessage("分类层级必须在1-10之间");
    }
}
