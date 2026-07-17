using FluentValidation;
using Wms.CycleCount.Application.Contracts.Dtos;

namespace Wms.CycleCount.Application.Contracts.Validators;

public class CycleCountPlanCreateDtoValidator : AbstractValidator<CycleCountPlanCreateDto>
{
    public CycleCountPlanCreateDtoValidator()
    {
        RuleFor(x => x.PlanNo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CountMethodValue).InclusiveBetween(1, 3);
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PlannedDate).NotEmpty();
        RuleFor(x => x.DifferenceThreshold).GreaterThan(0);
    }
}
