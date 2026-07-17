using FluentValidation;
using Wms.LineSide.Application.Contracts.Dtos;

namespace Wms.LineSide.Application.Contracts.Validators;

public class LineSideWarehouseCreateDtoValidator : AbstractValidator<LineSideWarehouseCreateDto>
{
    public LineSideWarehouseCreateDtoValidator()
    {
        RuleFor(x => x.LineSideWarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LineSideWarehouseName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductionLineId).NotEmpty();
        RuleFor(x => x.ProductionLineName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ConsumptionModeValue).InclusiveBetween(1, 2);
    }
}
