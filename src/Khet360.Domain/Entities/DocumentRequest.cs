using System;

namespace Khet360.Domain.Entities;

public class DocumentRequest : IBranchScoped
{
    public Guid Id { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public bool IsFulfilled { get; set; }

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
