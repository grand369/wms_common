using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wms.Production.Domain.Aggregates;

namespace Wms.Production.Domain.Repositories;

/// <summary>REP-16: SubcontractOrder repository (v2.0 placeholder)</summary>
public interface ISubcontractOrderRepository : IBasicRepository<SubcontractOrder, Guid>
{
    Task<SubcontractOrder?> FindByNoAsync(string orderNo);
    Task<List<SubcontractOrder>> GetByVendorAsync(Guid vendorId);
    Task<List<SubcontractOrder>> GetOverdueAsync();
}
