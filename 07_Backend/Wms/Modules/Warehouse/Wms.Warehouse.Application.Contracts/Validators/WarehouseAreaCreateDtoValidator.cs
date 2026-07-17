using FluentValidation;
using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Contracts.Validators;

/// <summary>
/// Warehouse Area Create DTO Validator — validates WarehouseAreaCreateDto using FluentValidation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class WarehouseAreaCreateDtoValidator : AbstractValidator<WarehouseAreaCreateDto>
{
    public WarehouseAreaCreateDtoValidator()
    {
        RuleFor(x => x.AreaCode)
            .NotEmpty().WithMessage("库区编码不能为空")
            .MaximumLength(50).WithMessage("库区编码长度不能超过50");

        RuleFor(x => x.AreaName)
            .NotEmpty().WithMessage("库区名称不能为空")
            .MaximumLength(200).WithMessage("库区名称长度不能超过200");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("所属仓库ID不能为空");

        RuleFor(x => x.WarehouseCode)
            .NotEmpty().WithMessage("所属仓库编码不能为空")
            .MaximumLength(50).WithMessage("仓库编码长度不能超过50");

        RuleFor(x => x.AreaFunction)
            .InclusiveBetween(0, 5).WithMessage("库区功能值必须在0-5之间");

        RuleFor(x => x.StorageEnvironment)
            .InclusiveBetween(0, 4).WithMessage("存储环境值必须在0-4之间");

        RuleFor(x => x.CurrentCapacity)
            .LessThanOrEqualTo(x => x.MaxCapacity).When(x => x.MaxCapacity != null && x.CurrentCapacity != null)
            .WithMessage("当前容量不能超过最大容量");
    }
}
