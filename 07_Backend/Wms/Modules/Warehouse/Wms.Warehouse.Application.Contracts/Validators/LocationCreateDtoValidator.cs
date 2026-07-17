using FluentValidation;
using Wms.Warehouse.Application.Contracts.Dtos;

namespace Wms.Warehouse.Application.Contracts.Validators;

/// <summary>
/// Location Create DTO Validator — validates LocationCreateDto using FluentValidation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class LocationCreateDtoValidator : AbstractValidator<LocationCreateDto>
{
    public LocationCreateDtoValidator()
    {
        RuleFor(x => x.LocationCode)
            .NotEmpty().WithMessage("库位编码不能为空")
            .MaximumLength(50).WithMessage("库位编码长度不能超过50");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("所属仓库ID不能为空");

        RuleFor(x => x.WarehouseCode)
            .NotEmpty().WithMessage("所属仓库编码不能为空")
            .MaximumLength(50).WithMessage("仓库编码长度不能超过50");

        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("所属库区ID不能为空");

        RuleFor(x => x.AreaCode)
            .NotEmpty().WithMessage("所属库区编码不能为空")
            .MaximumLength(50).WithMessage("库区编码长度不能超过50");

        RuleFor(x => x.BarcodeId)
            .NotEmpty().WithMessage("条码标识不能为空")
            .MaximumLength(100).WithMessage("条码标识长度不能超过100");

        RuleFor(x => x.LocationType)
            .InclusiveBetween(0, 4).WithMessage("库位类型值必须在0-4之间");

        RuleFor(x => x.StorageCondition)
            .InclusiveBetween(0, 4).WithMessage("存储条件值必须在0-4之间");
    }
}
