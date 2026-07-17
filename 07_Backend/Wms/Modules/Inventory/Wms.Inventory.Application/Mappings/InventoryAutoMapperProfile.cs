using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Application.Contracts.Dtos;
using AutoMapper;

namespace Wms.Inventory.Application.Mappings;

/// <summary>
/// Inventory Auto Mapper Profile — defines mappings between domain entities and DTOs.
/// </summary>
public class InventoryAutoMapperProfile : Profile
{
    public InventoryAutoMapperProfile()
    {
        CreateMap<InventoryBalance, InventoryBalanceOutputDto>()
            .ForMember(d => d.InventoryStatusValue, opt => opt.MapFrom(s => s.InventoryStatus.Value))
            .ForMember(d => d.InventoryStatusName, opt => opt.MapFrom(s => s.InventoryStatus.Description));

        CreateMap<InventoryLedgerEntry, InventoryLedgerOutputDto>()
            .ForMember(d => d.OperationTypeValue, opt => opt.MapFrom(s => s.OperationType.Value))
            .ForMember(d => d.OperationTypeName, opt => opt.MapFrom(s => s.OperationType.Description));

        CreateMap<InventoryAdjustment, InventoryAdjustmentOutputDto>()
            .ForMember(d => d.AdjustmentTypeValue, opt => opt.MapFrom(s => s.AdjustmentType.Value))
            .ForMember(d => d.AdjustmentTypeName, opt => opt.MapFrom(s => s.AdjustmentType.Description))
            .ForMember(d => d.ApprovalStatusValue, opt => opt.MapFrom(s => s.ApprovalStatus.Value))
            .ForMember(d => d.ApprovalStatusName, opt => opt.MapFrom(s => s.ApprovalStatus.Description));

        CreateMap<InventoryAdjustmentLine, InventoryAdjustmentLineDto>()
            .ForMember(d => d.InventoryStatusBeforeValue, opt => opt.MapFrom(s => s.InventoryStatusBefore.Value))
            .ForMember(d => d.InventoryStatusAfterValue, opt => opt.MapFrom(s => s.InventoryStatusAfter.Value));

        CreateMap<InventoryFreezeOrder, InventoryFreezeOutputDto>()
            .ForMember(d => d.FreezeScopeValue, opt => opt.MapFrom(s => s.FreezeScope.Value))
            .ForMember(d => d.FreezeScopeName, opt => opt.MapFrom(s => s.FreezeScope.Description))
            .ForMember(d => d.FreezeStatusValue, opt => opt.MapFrom(s => s.FreezeStatus.Value))
            .ForMember(d => d.FreezeStatusName, opt => opt.MapFrom(s => s.FreezeStatus.Description));

        CreateMap<InventoryAlert, InventoryAlertOutputDto>()
            .ForMember(d => d.AlertTypeValue, opt => opt.MapFrom(s => s.AlertType.Value))
            .ForMember(d => d.AlertTypeName, opt => opt.MapFrom(s => s.AlertType.Description));
    }
}
