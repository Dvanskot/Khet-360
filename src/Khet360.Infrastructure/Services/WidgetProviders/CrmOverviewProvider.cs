using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services.WidgetProviders;

public class CrmOverviewProvider : IWidgetProvider
{
    private readonly TenantDbContext _db;

    public string WidgetId => "crm-overview";

    public CrmOverviewProvider(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetDataAsync(Guid tenantId, Guid userId)
    {
        var newLeads = await _db.Leads.CountAsync(l => l.Status == LeadStatus.New);
        var openOpportunities = await _db.Opportunities
            .CountAsync(o => o.Stage != OpportunityStage.ClosedWon && o.Stage != OpportunityStage.ClosedLost);
        var pipelineValue = await _db.Opportunities
            .Where(o => o.Stage != OpportunityStage.ClosedWon && o.Stage != OpportunityStage.ClosedLost)
            .SumAsync(o => o.EstimatedValue);
        var totalLeads = await _db.Leads.CountAsync();
        var convertedLeads = await _db.Leads.CountAsync(l => l.Status == LeadStatus.Converted);
        double conversionRate = totalLeads == 0 ? 0 : (double)convertedLeads / totalLeads * 100;

        return new
        {
            NewLeads = newLeads,
            OpenOpportunities = openOpportunities,
            PipelineValue = pipelineValue,
            ConversionRate = conversionRate
        };
    }
}
