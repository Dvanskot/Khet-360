using Khet360.Application.Dtos;
using System.Threading.Tasks;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public interface IProductivityScorecardService
{
    Task<ProductivityScorecardDto> GetScorecardAsync(Guid branchId);
}
