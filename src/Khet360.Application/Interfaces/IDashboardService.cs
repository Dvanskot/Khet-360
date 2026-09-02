using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IDashboardService
{
    Task<Dictionary<string, object>> GetDashboardDataAsync(Guid userId);
    Task<UserDashboardLayoutDto> GetUserLayoutAsync(Guid userId);
    Task SaveUserLayoutAsync(UserDashboardLayoutDto layout);
}
