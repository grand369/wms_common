using FluentValidation;
using Wms.Material.Application.Contracts.Dtos;

namespace Wms.Material.Application.Contracts.Validators;

/// <summary>
/// Material Create DTO Validator — validates MaterialCreateDto using FluentValidation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class MaterialCreateDtoValidator : AbstractValidator<MaterialCreateDto>
{
    public MaterialCreateDtoValidator()
    {
        RuleFor(x => x.MaterialCode).NotEmpty().WithMessage("物料编码不能为空").MaximumLength(50);
        RuleFor(x => x.MaterialName).NotEmpty().WithMessage("物料名称不能为空").MaximumLength(200);
        RuleFor(x => x.MaterialType).InclusiveBetween(0, 7).WithMessage("物料类型值必须在0-7之间");
        RuleFor(x => x.PrimaryUnitId).NotEmpty().WithMessage("主计量单位ID不能为空");
        RuleFor(x => x.PrimaryUnitName).NotEmpty().WithMessage("主计量单位名称不能为空").MaximumLength(50);
        RuleFor(x => x.StorageConditionType).InclusiveBetween(0, 4).WithMessage("存储条件类型值必须在0-4之间");
        RuleFor(x => x.QualityInspectionMode).InclusiveBetween(0, 2).WithMessage("质检模式值必须在0-2之间");
        RuleFor(x => x.ABCClassification).InclusiveBetween(0, 2).WithMessage("ABC分类值必须在0-2之间");
        RuleFor(x => x.IssueStrategyType).InclusiveBetween(0, 3).WithMessage("发料策略值必须在0-3之间");
        RuleFor(x => x.StrategyScope).InclusiveBetween(0, 2).WithMessage("策略范围值必须在0-2之间");
        RuleFor(x => x.DangerLevel).InclusiveBetween(0, 4).WithMessage("危险等级值必须在0-4之间");
        RuleFor(x => x.ErpSyncStatus).InclusiveBetween(0, 3).WithMessage("ERP同步状态值必须在0-3之间");
    }
}
