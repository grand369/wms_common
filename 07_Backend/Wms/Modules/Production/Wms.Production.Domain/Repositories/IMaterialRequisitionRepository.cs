using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.Production.Domain.Aggregates;
using Wms.Production.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;

namespace Wms.Production.Domain.Repositories;

/// <summary>REP-15: MaterialRequisition repository</summary>
public interface IMaterialRequisitionRepository : IBasicRepository<MaterialRequisition, Guid>
{
    Task<MaterialRequisition?> FindByNoAsync(string requisitionNo);
    Task<List<MaterialRequisition>> GetByProductionOrderAsync(Guid productionOrderId);
    Task<MaterialRequisition> GetWithLinesAsync(Guid id);
}
