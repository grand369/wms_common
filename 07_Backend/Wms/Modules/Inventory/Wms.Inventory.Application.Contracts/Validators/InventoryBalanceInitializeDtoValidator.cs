using FluentValidation;
using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Validators;

/// <summary>
/// Inventory Balance Initialize DTO Validator — validates initialization requests.
/// </summary>
public class InventoryBalanceInitializeDtoValidator : AbstractValidator<InventoryBalanceInitializeDto>
{
    public InventoryBalanceInitializeDtoValidator()
    {
        RuleFor(x => x.MaterialId).NotEmpty().WithMessage("MaterialId is required.");
        RuleFor(x => x.MaterialCode).NotEmpty().MaximumLength(50).WithMessage("MaterialCode is required and max 50 chars.");
        RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("WarehouseId is required.");
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50).WithMessage("WarehouseCode is required and max 50 chars.");
        RuleFor(x => x.LocationId).NotEmpty().WithMessage("LocationId is required.");
        RuleFor(x => x.LocationCode).NotEmpty().MaximumLength(50).WithMessage("LocationCode is required and max 50 chars.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be >= 0 for initialization.");
        RuleFor(x => x.SourceOrderType).NotEmpty().WithMessage("SourceOrderType is required.");
    }
}
