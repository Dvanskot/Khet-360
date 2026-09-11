using System.Collections.Generic;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IRoutingService
{
    /// <summary>
    /// Finds the best available user to handle a work item based on
    /// role requirements and current workload.
    /// </summary>
    Task<Guid?> FindBestUserAsync(string entityType, Guid branchId);
}
