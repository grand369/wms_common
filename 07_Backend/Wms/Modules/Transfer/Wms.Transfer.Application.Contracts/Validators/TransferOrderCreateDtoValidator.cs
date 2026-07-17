using System;
using System.Collections.Generic;
using FluentValidation;
using Wms.Transfer.Application.Contracts.Dtos;

namespace Wms.Transfer.Application.Contracts.Validators;

/// <summary>Validator for TransferOrderCreateDto</summary>
public class TransferOrderCreateDtoValidator : AbstractValidator<TransferOrderCreateDto>
{
    public TransferOrderCreateDtoValidator()
    {
        RuleFor(x => x.TransferOrderNo)
            .NotEmpty().WithMessage("Transfer order number is required.")
            .MaximumLength(50);

        RuleFor(x => x.TransferTypeValue)
            .InclusiveBetween(1, 4).WithMessage("Transfer type value must be 1-4.");

        RuleFor(x => x.SourceWarehouseId).NotEmpty();
        RuleFor(x => x.SourceWarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TargetWarehouseId).NotEmpty();
        RuleFor(x => x.TargetWarehouseCode).NotEmpty().MaximumLength(50);

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("Transfer order must have at least one line.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.MaterialId).NotEmpty();
            line.RuleFor(l => l.MaterialCode).NotEmpty().MaximumLength(50);
            line.RuleFor(l => l.TransferQuantity).GreaterThan(0);
        });

        RuleFor(x => x.SourceWarehouseId)
            .NotEqual(x => x.TargetWarehouseId)
            .WithMessage("Source and target warehouse cannot be the same.");
    }
}
