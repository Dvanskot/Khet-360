using Khet360.Application.Dtos;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IProductivityScorecardService
{
    Task<ProductivityScorecardDto> GetScorecardAsync(Guid branchId);
}
