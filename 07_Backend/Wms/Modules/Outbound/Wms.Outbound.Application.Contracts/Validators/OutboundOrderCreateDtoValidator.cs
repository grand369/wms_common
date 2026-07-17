using FluentValidation;
using Wms.Outbound.Application.Contracts.Dtos;

namespace Wms.Outbound.Application.Contracts.Validators;

/// <summary>
/// OutboundOrderCreateDtoValidator — validates creation DTO.
/// Checks required fields, nested lines, and type-specific requirements.
/// </summary>
public class OutboundOrderCreateDtoValidator : AbstractValidator<OutboundOrderCreateDto>
{
    public OutboundOrderCreateDtoValidator()
    {
        RuleFor(x => x.OutboundTypeValue).InclusiveBetween(1, 4)
            .WithMessage("OutboundType must be between 1 (MaterialRequisition) and 4 (TransferOutbound).");

        RuleFor(x => x.WarehouseId).NotEmpty()
            .WithMessage("WarehouseId is required.");

        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50)
            .WithMessage("WarehouseCode is required and must not exceed 50 characters.");

        RuleFor(x => x.OverIssueRatio).InclusiveBetween(0m, 1m)
            .WithMessage("OverIssueRatio must be between 0 and 1.");

        RuleFor(x => x.Lines).NotEmpty()
            .WithMessage("At least one outbound line is required.");

        RuleForEach(x => x.Lines).SetValidator(new OutboundLineCreateDtoValidator());

        // Type-specific validation
        When(x => x.OutboundTypeValue == 1, () =>
        {
            RuleFor(x => x.MaterialRequisitionId).NotNull()
                .WithMessage("MaterialRequisitionId is required for MaterialRequisition outbound type.");
        });

        When(x => x.OutboundTypeValue == 2, () =>
        {
            RuleFor(x => x.SalesOrderId).NotNull()
                .WithMessage("SalesOrderId is required for SalesShipment outbound type.");
        });

        When(x => x.OutboundTypeValue == 3, () =>
        {
            RuleFor(x => x.ReturnMaterialOrderId).NotNull()
                .WithMessage("ReturnMaterialOrderId is required for ReturnMaterial outbound type.");
        });
    }
}

/// <summary>
/// OutboundLineCreateDtoValidator — validates a single outbound line creation DTO.
/// </summary>
public class OutboundLineCreateDtoValidator : AbstractValidator<OutboundLineCreateDto>
{
    public OutboundLineCreateDtoValidator()
    {
        RuleFor(x => x.MaterialId).NotEmpty()
            .WithMessage("MaterialId is required.");

        RuleFor(x => x.MaterialCode).NotEmpty().MaximumLength(50)
            .WithMessage("MaterialCode is required and must not exceed 50 characters.");

        RuleFor(x => x.MaterialName).NotEmpty().MaximumLength(200)
            .WithMessage("MaterialName is required and must not exceed 200 characters.");

        RuleFor(x => x.RequiredQuantity).GreaterThan(0m)
            .WithMessage("RequiredQuantity must be greater than 0.");

        RuleFor(x => x.IssueStrategyValue).InclusiveBetween(0, 3)
            .WithMessage("IssueStrategyValue must be between 0 (FIFO) and 3 (Manual).");
    }
}

/// <summary>
/// OutboundAllocateCommandDtoValidator — validates allocation command DTO.
/// </summary>
public class OutboundAllocateCommandDtoValidator : AbstractValidator<OutboundAllocateCommandDto>
{
    public OutboundAllocateCommandDtoValidator()
    {
        RuleFor(x => x.IdempotencyId).NotEmpty().MaximumLength(100)
            .WithMessage("IdempotencyId is required and must not exceed 100 characters.");

        RuleFor(x => x.Lines).NotEmpty()
            .WithMessage("At least one allocation line is required.");

        RuleForEach(x => x.Lines).SetValidator(new OutboundAllocateLineDtoValidator());
    }
}

/// <summary>
/// OutboundAllocateLineDtoValidator — validates a single allocation line.
/// </summary>
public class OutboundAllocateLineDtoValidator : AbstractValidator<OutboundAllocateLineDto>
{
    public OutboundAllocateLineDtoValidator()
    {
        RuleFor(x => x.LineId).NotEmpty()
            .WithMessage("LineId is required.");

        RuleFor(x => x.AllocatedQuantity).GreaterThan(0m)
            .WithMessage("AllocatedQuantity must be greater than 0.");
    }
}
