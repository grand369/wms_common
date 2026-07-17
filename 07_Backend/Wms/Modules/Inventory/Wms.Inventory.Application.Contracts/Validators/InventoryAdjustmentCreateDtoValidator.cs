using FluentValidation;
using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Validators;

/// <summary>
/// Inventory Adjustment Create DTO Validator — validates adjustment creation requests.
/// </summary>
public class InventoryAdjustmentCreateDtoValidator : AbstractValidator<InventoryAdjustmentCreateDto>
{
    public InventoryAdjustmentCreateDtoValidator()
    {
        RuleFor(x => x.AdjustmentNo).NotEmpty().MaximumLength(50).WithMessage("AdjustmentNo is required and max 50 chars.");
        RuleFor(x => x.AdjustmentReason).NotEmpty().MaximumLength(500).WithMessage("AdjustmentReason is required and max 500 chars.");
        RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("WarehouseId is required.");
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50).WithMessage("WarehouseCode is required.");
        RuleFor(x => x.Lines).NotEmpty().WithMessage("At least one adjustment line is required.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.MaterialId).NotEmpty().WithMessage("Line MaterialId is required.");
            line.RuleFor(l => l.MaterialCode).NotEmpty().WithMessage("Line MaterialCode is required.");
            line.RuleFor(l => l.LocationId).NotEmpty().WithMessage("Line LocationId is required.");
            line.RuleFor(l => l.LocationCode).NotEmpty().WithMessage("Line LocationCode is required.");
            line.RuleFor(l => l.AdjustmentQuantity).NotEqual(0).WithMessage("AdjustmentQuantity cannot be zero.");
        });
    }
}
