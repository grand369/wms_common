using FluentValidation;
using Wms.Notification.Application.Contracts.Dtos;

namespace Wms.Notification.Application.Contracts.Validators;

public class NotificationTemplateCreateDtoValidator : AbstractValidator<NotificationTemplateCreateDto>
{
    public NotificationTemplateCreateDtoValidator()
    {
        RuleFor(x => x.TemplateName)
            .NotEmpty().WithMessage("模板名称不能为空")
            .MaximumLength(100).WithMessage("模板名称最长100字符");

        RuleFor(x => x.TemplateTypeValue)
            .InclusiveBetween(0, 3).WithMessage("通知类型值必须在0~3之间");

        RuleFor(x => x.ChannelValue)
            .InclusiveBetween(0, 4).WithMessage("通知渠道值必须在0~4之间");

        RuleFor(x => x.TemplateContent)
            .NotEmpty().WithMessage("模板内容不能为空");
    }
}
