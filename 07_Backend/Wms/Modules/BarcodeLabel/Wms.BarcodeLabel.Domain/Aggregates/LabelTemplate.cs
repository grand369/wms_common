using System;
using Volo.Abp.Domain.Entities.Auditing;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Aggregates;

/// <summary>
/// LabelTemplate Aggregate Root (AGG-22) — defines label template configurations.
/// </summary>
public class LabelTemplate : FullAuditedAggregateRoot<Guid>
{
    public string TemplateName { get; private set; }
    public LabelTemplateType TemplateType { get; private set; }
    public string TemplateContent { get; private set; }
    public int TemplateVersion { get; private set; }
    public string? IndustryStandard { get; private set; }
    public bool IsActive { get; private set; }

    private LabelTemplate() { }

    public LabelTemplate(
        Guid id,
        string templateName,
        LabelTemplateType templateType,
        string templateContent,
        string? industryStandard = null)
        : base(id)
    {
        TemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
        TemplateType = templateType ?? throw new ArgumentNullException(nameof(templateType));
        TemplateContent = templateContent ?? throw new ArgumentNullException(nameof(templateContent));
        IndustryStandard = industryStandard;
        TemplateVersion = 1;
        IsActive = true;
    }

    /// <summary>Update the template content and increment the version.</summary>
    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new BusinessException("WMS:BarcodeLabel:InvalidTemplateContent",
                "Template content cannot be empty.");

        TemplateContent = newContent;
        IncrementVersion();
    }

    /// <summary>Increment the template version.</summary>
    public void IncrementVersion()
    {
        TemplateVersion++;
    }

    /// <summary>Deactivate this label template.</summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new BusinessException("WMS:BarcodeLabel:TemplateAlreadyInactive",
                $"Label template '{TemplateName}' is already inactive.");

        IsActive = false;
    }

    /// <summary>Update template name and industry standard.</summary>
    public void UpdateInfo(string templateName, string? industryStandard)
    {
        if (string.IsNullOrWhiteSpace(templateName))
            throw new BusinessException("WMS:BarcodeLabel:InvalidTemplateName",
                "Template name cannot be empty.");

        TemplateName = templateName;
        IndustryStandard = industryStandard;
    }

    /// <summary>Set active status.</summary>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}
