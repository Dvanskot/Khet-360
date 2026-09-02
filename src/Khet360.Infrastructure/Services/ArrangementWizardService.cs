using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class ArrangementWizardService : IArrangementWizardService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;
    private readonly IServiceArrangementService _arrangementService;

    public ArrangementWizardService(TenantDbContext db, ITenantService tenantService, IServiceArrangementService arrangementService)
    {
        _db = db;
        _tenantService = tenantService;
        _arrangementService = arrangementService;
    }

    public async Task<ArrangementWizardState> StartWizardAsync(Guid funeralCaseId, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var existing = await _db.ArrangementWizardStates
            .FirstOrDefaultAsync(s => s.FuneralCaseId == funeralCaseId && !s.IsCompleted);

        if (existing != null) return existing;

        var state = new ArrangementWizardState
        {
            Id = Guid.NewGuid(),
            FuneralCaseId = funeralCaseId,
            CurrentStep = (int)ArrangementWizardStep.BasicInfo,
            IsCompleted = false,
            LastUpdated = DateTime.UtcNow,
            FormDataJson = "{}",
            TenantId = tenantId,
            BranchId = branchId
        };

        _db.ArrangementWizardStates.Add(state);
        await _db.SaveChangesAsync();

        return state;
    }

    public async Task<WizardStepResult> SaveStepAsync(Guid stateId, ArrangementWizardStep step, Dictionary<string, string> data)
    {
        var state = await _db.ArrangementWizardStates.FindAsync(stateId);
        if (state == null) throw new KeyNotFoundException("Wizard state not found.");

        var errors = ValidateStepData(step, data);
        if (errors.Any())
        {
            return new WizardStepResult(false, "Validation failed", errors);
        }

        var currentData = JsonSerializer.Deserialize<Dictionary<string, string>>(state.FormDataJson) ?? new Dictionary<string, string>();
        foreach (var kvp in data)
        {
            currentData[kvp.Key] = kvp.Value;
        }

        state.FormDataJson = JsonSerializer.Serialize(currentData);
        state.CurrentStep = (int)step;
        state.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new WizardStepResult(true, "Step saved successfully", new List<string>());
    }

    public async Task<ArrangementWizardState> GetStateAsync(Guid stateId)
    {
        return await _db.ArrangementWizardStates.FindAsync(stateId);
    }

    public async Task FinalizeArrangementAsync(Guid stateId)
    {
        var state = await _db.ArrangementWizardStates.FindAsync(stateId);
        if (state == null) throw new KeyNotFoundException("Wizard state not found.");

        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(state.FormDataJson);

        var arrangement = new ServiceArrangement
        {
            Id = Guid.NewGuid(),
            FuneralCaseId = state.FuneralCaseId,
            TenantId = state.TenantId,
            BranchId = state.BranchId,
            ScheduledDate = DateTime.UtcNow
        };

        _db.ServiceArrangements.Add(arrangement);

        state.IsCompleted = true;
        state.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    private List<string> ValidateStepData(ArrangementWizardStep step, Dictionary<string, string> data)
    {
        var errors = new List<string>();

        switch (step)
        {
            case ArrangementWizardStep.BasicInfo:
                if (!data.ContainsKey("ServiceDate")) errors.Add("Service date is required.");
                if (!data.ContainsKey("Location")) errors.Add("Location is required.");
                break;
            case ArrangementWizardStep.CasketSelection:
                if (!data.ContainsKey("CasketId")) errors.Add("Casket selection is required.");
                break;
        }

        return errors;
    }
}
