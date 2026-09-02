using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Application.Dtos;

public record ArrangementWizardStateDto(
    Guid FuneralCaseId,
    ArrangementWizardStep CurrentStep,
    bool IsCompleted,
    DateTime LastUpdated,
    Dictionary<string, string> FormData);

public record WizardStepResult(
    bool Success,
    string Message,
    List<string> ValidationErrors);
