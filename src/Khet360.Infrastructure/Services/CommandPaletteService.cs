using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class CommandPaletteService : ICommandPaletteService
{
    private readonly ITenantService _tenantService;
    private readonly IAuthorizationService _authService;

    public CommandPaletteService(ITenantService tenantService, IAuthorizationService authService)
    {
        _tenantService = tenantService;
        _authService = authService;
    }

    public async Task<List<CommandActionDto>> GetAvailableCommandsAsync(string context = null)
    {
        var commands = new List<CommandActionDto>();

        // --- Global Commands ---
        commands.Add(new CommandActionDto(
            "nav-dashboard",
            "Go to Dashboard",
            "View operational overview",
            "Navigation",
            CommandActionType.Navigate,
            "/dashboard"));

        commands.Add(new CommandActionDto(
            "nav-cases",
            "View All Cases",
            "Open the funeral cases list",
            "Navigation",
            CommandActionType.Navigate,
            "/cases"));

        commands.Add(new CommandActionDto(
            "create-lead",
            "Quick Create Lead",
            "Open lead creation form",
            "CRM",
            CommandActionType.OpenModal,
            "lead-creation-modal"));

        // --- Contextual Commands ---
        if (context == "FuneralCase")
        {
            commands.Add(new CommandActionDto(
                "case-close",
                "Close Case",
                "Mark current case as completed",
                "Case Management",
                CommandActionType.ExecuteApi,
                "/api/cases/close"));

            commands.Add(new CommandActionDto(
                "case-assign-driver",
                "Assign Driver",
                "Dispatch a vehicle for this case",
                "Logistics",
                CommandActionType.OpenModal,
                "driver-assignment-modal"));

            commands.Add(new CommandActionDto(
                "case-generate-invoice",
                "Generate Invoice",
                "Create final billing for the family",
                "Financials",
                CommandActionType.ExecuteApi,
                "/api/payments/invoice"));
        }
        else if (context == "Lead")
        {
            commands.Add(new CommandActionDto(
                "lead-convert",
                "Convert to Customer",
                "Promote lead to a customer and create opportunity",
                "CRM",
                CommandActionType.ExecuteApi,
                "/api/leads/convert"));
        }
        else if (context == "Fleet")
        {
            commands.Add(new CommandActionDto(
                "fleet-maintenance",
                "Schedule Maintenance",
                "Create a work order for vehicle repair",
                "Logistics",
                CommandActionType.OpenModal,
                "maintenance-modal"));
        }

        // Filter based on permissions (Mocked here, but would use _authService)
        // In a real app, each CommandActionDto would have a 'RequiredPermission' property.

        return commands;
    }
}
