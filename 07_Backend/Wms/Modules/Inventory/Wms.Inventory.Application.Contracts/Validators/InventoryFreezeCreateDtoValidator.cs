using FluentValidation;
using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Validators;

/// <summary>
/// Inventory Freeze Create DTO Validator — validates freeze order creation requests.
/// </summary>
public class InventoryFreezeCreateDtoValidator : AbstractValidator<InventoryFreezeCreateDto>
{
    public InventoryFreezeCreateDtoValidator()
    {
        RuleFor(x => x.FreezeOrderNo).NotEmpty().MaximumLength(50).WithMessage("FreezeOrderNo is required and max 50 chars.");
        RuleFor(x => x.FreezeReason).NotEmpty().MaximumLength(500).WithMessage("FreezeReason is required and max 500 chars.");
        RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("WarehouseId is required.");
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50).WithMessage("WarehouseCode is required.");
        RuleFor(x => x.FreezeStartTime).NotEmpty().WithMessage("FreezeStartTime is required.");
        RuleFor(x => x.FreezeRanges).NotEmpty().WithMessage("At least one freeze range is required.");
    }
}
