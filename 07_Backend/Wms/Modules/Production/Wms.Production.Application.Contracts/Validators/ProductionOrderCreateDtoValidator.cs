using FluentValidation;
using Wms.Production.Application.Contracts.Dtos;

namespace Wms.Production.Application.Contracts.Validators;

public class ProductionOrderCreateDtoValidator : AbstractValidator<ProductionOrderCreateDto>
{
    public ProductionOrderCreateDtoValidator()
    {
        RuleFor(x => x.ProductionOrderNo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.MaterialCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PlanQuantity).GreaterThan(0);
        RuleFor(x => x.PlannedStartDate).NotEmpty();
        RuleFor(x => x.PlannedEndDate).NotEmpty().GreaterThan(x => x.PlannedStartDate);
    }
}
