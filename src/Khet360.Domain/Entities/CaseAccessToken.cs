using System;

namespace Khet360.Domain.Entities;

public class CaseAccessToken : IBranchScoped
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid BranchId { get; set; }
}
