using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class ArrangementWizardState : IBranchScoped
{
    public Guid Id { get; set; }
    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public int CurrentStep { get; set; } // Cast to ArrangementWizardStep
    public bool IsCompleted { get; set; }
    public DateTime LastUpdated { get; set; }

    // Store form data as JSON
    public string FormDataJson { get; set; } = "{}";

    public Guid BranchId { get; set; }
}
