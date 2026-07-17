using AutoMapper;
using Wms.RuleEngine.Application.Contracts.Dtos;
using Wms.RuleEngine.Domain.Aggregates;

namespace Wms.RuleEngine.Application.Mappings;

/// <summary>
/// RuleEngine AutoMapper Profile — configures Domain → DTO mappings for BusinessRule and IndustryPackage.
/// </summary>
public class RuleEngineAutoMapperProfile : Profile
{
    public RuleEngineAutoMapperProfile()
    {
        // BusinessRule → BusinessRuleOutputDto
        CreateMap<BusinessRule, BusinessRuleOutputDto>();

        // IndustryPackage → IndustryPackageOutputDto
        CreateMap<IndustryPackage, IndustryPackageOutputDto>();
    }
}
