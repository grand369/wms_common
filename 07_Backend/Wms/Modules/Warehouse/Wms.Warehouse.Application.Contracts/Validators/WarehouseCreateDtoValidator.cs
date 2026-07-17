using FluentValidation;
using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Contracts.Validators;

/// <summary>
/// Warehouse Create DTO Validator — validates WarehouseCreateDto using FluentValidation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class WarehouseCreateDtoValidator : AbstractValidator<WarehouseCreateDto>
{
    public WarehouseCreateDtoValidator()
    {
        RuleFor(x => x.WarehouseCode)
            .NotEmpty().WithMessage("仓库编码不能为空")
            .MaximumLength(50).WithMessage("仓库编码长度不能超过50")
            .Matches(@"^[A-Z0-9_-]+$").WithMessage("仓库编码只能包含大写字母、数字、下划线和连字符");

        RuleFor(x => x.WarehouseName)
            .NotEmpty().WithMessage("仓库名称不能为空")
            .MaximumLength(200).WithMessage("仓库名称长度不能超过200");

        RuleFor(x => x.WarehouseType)
            .InclusiveBetween(0, 11).WithMessage("仓库类型值必须在0-11之间");

        RuleFor(x => x.OrganizationUnitId)
            .NotEmpty().WithMessage("组织单元ID不能为空");

        RuleFor(x => x.OrganizationUnitName)
            .NotEmpty().WithMessage("组织名称不能为空")
            .MaximumLength(200).WithMessage("组织名称长度不能超过200");

        RuleFor(x => x.PlantId)
            .NotEmpty().WithMessage("工厂ID不能为空");

        RuleFor(x => x.PlantName)
            .NotEmpty().WithMessage("工厂名称不能为空")
            .MaximumLength(100).WithMessage("工厂名称长度不能超过100");

        RuleFor(x => x.LocationLevelCount)
            .InclusiveBetween(3, 4).WithMessage("库位层级数必须为3或4");

        RuleFor(x => x.StorageConditionType)
            .InclusiveBetween(0, 4).WithMessage("存储条件类型值必须在0-4之间");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("地址长度不能超过500");

        RuleFor(x => x.Remark)
            .MaximumLength(1000).WithMessage("备注长度不能超过1000");
    }
}
