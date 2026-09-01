using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record OperationalDashboardDto(
    SlaOverviewDto SlaOverview,
    FleetOverviewDto FleetOverview,
    VendorOverviewDto VendorOverview,
    CrmOverviewDto CrmOverview);

public record SlaOverviewDto(
    int TotalOpenItems,
    int WarningItems,
    int BreachedItems,
    List<SlaAlertDto> CriticalAlerts);

public record SlaAlertDto(
    Guid ItemId,
    string ItemType,
    string Description,
    DateTime Deadline,
    string Severity);

public record FleetOverviewDto(
    int TotalVehicles,
    int ActiveVehicles,
    int IdleVehicles,
    int MaintenanceDue,
    decimal AverageFuelEfficiency);

public record VendorOverviewDto(
    int TotalPendingOrders,
    int OverdueOrders,
    decimal TotalPendingValue);

public record CrmOverviewDto(
    int NewLeads,
    int OpenOpportunities,
    decimal PipelineValue,
    double ConversionRate);
