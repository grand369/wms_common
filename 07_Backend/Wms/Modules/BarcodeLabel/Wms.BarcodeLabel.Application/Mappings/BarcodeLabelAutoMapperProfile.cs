using Mapster;
using Wms.BarcodeLabel.Application.Contracts.Dtos;
using Wms.BarcodeLabel.Domain.Aggregates;

namespace Wms.BarcodeLabel.Application.Mappings;

/// <summary>
/// BarcodeLabel Mapster Mapper Profile — defines mappings between domain entities and DTOs.
/// </summary>
public static class BarcodeLabelAutoMapperProfile
{
    public static void Configure()
    {
        TypeAdapterConfig<BarcodeRule, BarcodeRuleOutputDto>
            .NewConfig()
            .Map(d => d.BarcodeTypeValue, s => s.BarcodeType.Value)
            .Map(d => d.BarcodeTypeName, s => s.BarcodeType.Description)
            .Map(d => d.BarcodeFormatValue, s => s.BarcodeFormat.Value)
            .Map(d => d.BarcodeFormatName, s => s.BarcodeFormat.Description);

        TypeAdapterConfig<LabelTemplate, LabelTemplateOutputDto>
            .NewConfig()
            .Map(d => d.TemplateTypeValue, s => s.TemplateType.Value)
            .Map(d => d.TemplateTypeName, s => s.TemplateType.Description);

        TypeAdapterConfig<PrintTask, PrintTaskOutputDto>
            .NewConfig()
            .Map(d => d.PrintStatusValue, s => s.PrintStatus.Value)
            .Map(d => d.PrintStatusName, s => s.PrintStatus.Description);
    }
}
