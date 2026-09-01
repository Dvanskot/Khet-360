using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IDashboardService
{
    Task<OperationalDashboardDto> GetOperationalOverviewAsync();
}
