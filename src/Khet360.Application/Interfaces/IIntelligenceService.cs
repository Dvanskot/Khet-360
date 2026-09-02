using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IIntelligenceService
{
    Task<PlatformHealthDto> GetPlatformHealthAsync();
    Task<List<TenantGrowthDto>> GetTenantGrowthTrendsAsync(DateTime from, DateTime to);
    Task<List<FeatureUsageDto>> GetGlobalFeatureUsageAsync();
}
