using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Wms.TaskCenter.Application.Contracts.Dtos;

namespace Wms.TaskCenter.Application.Contracts.Validators;

/// <summary>
/// Validator for WarehouseTaskCreateDto — API-TC-003.
/// </summary>
public class WarehouseTaskCreateDtoValidator : AbstractValidator<WarehouseTaskCreateDto>
{
    public WarehouseTaskCreateDtoValidator()
    {
        RuleFor(x => x.TaskNo)
            .NotEmpty().WithMessage("任务编号不能为空")
            .MaximumLength(50).WithMessage("任务编号最长50字符");

        RuleFor(x => x.TaskTypeValue)
            .InclusiveBetween(1, 8).WithMessage("任务类型值必须在1~8之间");

        RuleFor(x => x.TaskPriorityValue)
            .InclusiveBetween(1, 4).WithMessage("任务优先级值必须在1~4之间");

        RuleFor(x => x.SourceOrderType)
            .NotEmpty().WithMessage("来源单据类型不能为空")
            .MaximumLength(50);

        RuleFor(x => x.SourceOrderId)
            .NotEqual(Guid.Empty).WithMessage("来源单据ID不能为空");

        RuleFor(x => x.SourceOrderNo)
            .NotEmpty().WithMessage("来源单据号不能为空")
            .MaximumLength(50);

        RuleFor(x => x.WarehouseId)
            .NotEqual(Guid.Empty).WithMessage("仓库ID不能为空");

        RuleFor(x => x.WarehouseCode)
            .NotEmpty().WithMessage("仓库编码不能为空")
            .MaximumLength(50);

        RuleFor(x => x.AssignmentStrategyValue)
            .InclusiveBetween(0, 3).WithMessage("分配策略值必须在0~3之间");
    }
}

/// <summary>
/// Validator for TaskSuspendCommandDto — API-TC-007.
/// TC-004: SuspendedReason is required.
/// </summary>
public class TaskSuspendCommandDtoValidator : AbstractValidator<TaskSuspendCommandDto>
{
    public TaskSuspendCommandDtoValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("挂起原因不能为空 (TC-004)")
            .MaximumLength(500);
    }
}

/// <summary>
/// Validator for TaskUpdateProgressCommandDto — API-TC-013.
/// </summary>
public class TaskUpdateProgressCommandDtoValidator : AbstractValidator<TaskUpdateProgressCommandDto>
{
    public TaskUpdateProgressCommandDtoValidator()
    {
        RuleFor(x => x.Progress)
            .InclusiveBetween(0, 100).WithMessage("进度必须在0~100之间");
    }
}

/// <summary>
/// Validator for TaskBatchAssignCommandDto — API-TC-012.
/// </summary>
public class TaskBatchAssignCommandDtoValidator : AbstractValidator<TaskBatchAssignCommandDto>
{
    public TaskBatchAssignCommandDtoValidator()
    {
        RuleFor(x => x.TaskIds)
            .NotEmpty().WithMessage("任务ID列表不能为空");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("操作员ID不能为空");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("操作员名称不能为空")
            .MaximumLength(100);
    }
}
