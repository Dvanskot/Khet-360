using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public interface IArrangementWizardService
{
    Task<ArrangementWizardState> StartWizardAsync(Guid funeralCaseId, Guid branchId);
    Task<WizardStepResult> SaveStepAsync(Guid stateId, ArrangementWizardStep step, Dictionary<string, string> data);
    Task<ArrangementWizardState?> GetStateAsync(Guid stateId);
    Task FinalizeArrangementAsync(Guid stateId);
}
