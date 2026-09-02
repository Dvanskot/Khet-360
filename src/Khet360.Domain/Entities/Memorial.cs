using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class Memorial : IBranchScoped
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public MemorialType Type { get; set; }
    public string? Theme { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? PublicUrl { get; set; }

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid BranchId { get; set; }

    public virtual Obituary? Obituary { get; set; }
    public virtual ICollection<MemorialTribute> Tributes { get; set; } = new List<MemorialTribute>();
}

public class Obituary : IBranchScoped
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public ObituaryStatus Status { get; set; }
    public DateTime PublishedAt { get; set; }

    public Guid MemorialId { get; set; }
    public virtual Memorial Memorial { get; set; } = null!;

    public Guid BranchId { get; set; }
}

public class MemorialTribute : IBranchScoped
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
    public bool IsApproved { get; set; } = false;

    public Guid MemorialId { get; set; }
    public virtual Memorial Memorial { get; set; } = null!;

    public Guid BranchId { get; set; }
}
