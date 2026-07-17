using AutoMapper;
using Wms.TaskCenter.Domain.Aggregates;
using Wms.TaskCenter.Application.Contracts.Dtos;

namespace Wms.TaskCenter.Application.Mappings;

/// <summary>
/// AutoMapper profile for WarehouseTask — SmartEnum → Value + Description.
/// </summary>
public class WarehouseTaskAutoMapperProfile : Profile
{
    public WarehouseTaskAutoMapperProfile()
    {
        CreateMap<WarehouseTask, WarehouseTaskOutputDto>()
            .ForMember(d => d.TaskTypeValue, opt => opt.MapFrom(s => s.TaskType.Value))
            .ForMember(d => d.TaskTypeDescription, opt => opt.MapFrom(s => s.TaskType.Description))
            .ForMember(d => d.TaskPriorityValue, opt => opt.MapFrom(s => s.TaskPriority.Value))
            .ForMember(d => d.TaskPriorityDescription, opt => opt.MapFrom(s => s.TaskPriority.Description))
            .ForMember(d => d.TaskStatusValue, opt => opt.MapFrom(s => s.TaskStatus.Value))
            .ForMember(d => d.TaskStatusDescription, opt => opt.MapFrom(s => s.TaskStatus.Description))
            .ForMember(d => d.AssignmentStrategyValue, opt => opt.MapFrom(s => s.AssignmentStrategy.Value))
            .ForMember(d => d.AssignmentStrategyDescription, opt => opt.MapFrom(s => s.AssignmentStrategy.Description));
    }
}
