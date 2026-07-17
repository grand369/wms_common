using FluentValidation;
using Wms.Notification.Application.Contracts.Dtos;

namespace Wms.Notification.Application.Contracts.Validators;

public class NotificationRuleCreateDtoValidator : AbstractValidator<NotificationRuleCreateDto>
{
    public NotificationRuleCreateDtoValidator()
    {
        RuleFor(x => x.RuleName)
            .NotEmpty().WithMessage("规则名称不能为空")
            .MaximumLength(100).WithMessage("规则名称最长100字符");

        RuleFor(x => x.SourceEvent)
            .NotEmpty().WithMessage("源事件不能为空")
            .MaximumLength(200).WithMessage("源事件最长200字符");

        RuleFor(x => x.TargetChannelValue)
            .InclusiveBetween(0, 4).WithMessage("目标渠道值必须在0~4之间");
    }
}
