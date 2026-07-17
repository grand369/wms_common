using FluentValidation;
using Wms.Inbound.Application.Contracts.Dtos;

namespace Wms.Inbound.Application.Contracts.Validators;

/// <summary>
/// InboundOrderCreateDtoValidator — validates creation DTO.
/// Checks required fields, nested lines, and type-specific requirements.
/// </summary>
public class InboundOrderCreateDtoValidator : AbstractValidator<InboundOrderCreateDto>
{
    public InboundOrderCreateDtoValidator()
    {
        RuleFor(x => x.InboundTypeValue).InclusiveBetween(1, 4)
            .WithMessage("InboundType must be between 1 (PurchaseReceipt) and 4 (TransferInbound).");

        RuleFor(x => x.WarehouseId).NotEmpty()
            .WithMessage("WarehouseId is required.");

        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50)
            .WithMessage("WarehouseCode is required and must not exceed 50 characters.");

        RuleFor(x => x.OverReceiptRatio).InclusiveBetween(0m, 1m)
            .WithMessage("OverReceiptRatio must be between 0 and 1.");

        RuleFor(x => x.Lines).NotEmpty()
            .WithMessage("At least one inbound line is required.");

        RuleForEach(x => x.Lines).SetValidator(new InboundLineCreateDtoValidator());

        // Type-specific validation
        When(x => x.InboundTypeValue == 1, () =>
        {
            RuleFor(x => x.PurchaseOrderId).NotNull()
                .WithMessage("PurchaseOrderId is required for PurchaseReceipt inbound type.");
            RuleFor(x => x.SupplierId).NotNull()
                .WithMessage("SupplierId is required for PurchaseReceipt inbound type.");
        });

        When(x => x.InboundTypeValue == 2, () =>
        {
            RuleFor(x => x.ProductionOrderId).NotNull()
                .WithMessage("ProductionOrderId is required for ProductionReceipt inbound type.");
        });

        When(x => x.InboundTypeValue == 3, () =>
        {
            RuleFor(x => x.ReturnOrderId).NotNull()
                .WithMessage("ReturnOrderId is required for ReturnReceipt inbound type.");
        });
    }
}

/// <summary>
/// InboundLineCreateDtoValidator — validates a single inbound line creation DTO.
/// </summary>
public class InboundLineCreateDtoValidator : AbstractValidator<InboundLineCreateDto>
{
    public InboundLineCreateDtoValidator()
    {
        RuleFor(x => x.MaterialId).NotEmpty()
            .WithMessage("MaterialId is required.");

        RuleFor(x => x.MaterialCode).NotEmpty().MaximumLength(50)
            .WithMessage("MaterialCode is required and must not exceed 50 characters.");

        RuleFor(x => x.MaterialName).NotEmpty().MaximumLength(200)
            .WithMessage("MaterialName is required and must not exceed 200 characters.");

        RuleFor(x => x.PlanQuantity).GreaterThan(0m)
            .WithMessage("PlanQuantity must be greater than 0.");
    }
}
