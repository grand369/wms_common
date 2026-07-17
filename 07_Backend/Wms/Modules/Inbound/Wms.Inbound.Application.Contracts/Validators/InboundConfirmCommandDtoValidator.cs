using FluentValidation;
using Wms.Inbound.Application.Contracts.Dtos;

namespace Wms.Inbound.Application.Contracts.Validators;

/// <summary>
/// InboundConfirmCommandDtoValidator — validates receipt confirmation DTO.
/// Checks idempotency ID and line-level quantities.
/// </summary>
public class InboundConfirmCommandDtoValidator : AbstractValidator<InboundConfirmCommandDto>
{
    public InboundConfirmCommandDtoValidator()
    {
        RuleFor(x => x.IdempotencyId).NotEmpty().MaximumLength(100)
            .WithMessage("IdempotencyId is required and must not exceed 100 characters.");

        RuleFor(x => x.Lines).NotEmpty()
            .WithMessage("At least one confirmation line is required.");

        RuleForEach(x => x.Lines).SetValidator(new InboundConfirmLineDtoValidator());
    }
}

/// <summary>
/// InboundConfirmLineDtoValidator — validates a single confirmation line.
/// </summary>
public class InboundConfirmLineDtoValidator : AbstractValidator<InboundConfirmLineDto>
{
    public InboundConfirmLineDtoValidator()
    {
        RuleFor(x => x.LineId).NotEmpty()
            .WithMessage("LineId is required.");

        RuleFor(x => x.ReceivedQuantity).GreaterThan(0m)
            .WithMessage("ReceivedQuantity must be greater than 0.");
    }
}
